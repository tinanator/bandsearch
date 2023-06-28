using System.Threading;
using BandSearch.Database;
using BandSearch.Models;
using BandSearch.Web.Database;
using BandSearch.Web.Exceptions;
using BandSearch.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BandSearch.Web.Services
{
    public class BandService : IBandService
    {
        private static readonly SemaphoreSlim s_semaphore = new SemaphoreSlim(1, 1);
        private readonly int _cacheSlidingExpirationTime = 60;
        private readonly int _cacheAbsoluteExpirationTime = 120;
        private readonly int _cacheEntrySize = 1;

        private IBandRepository _bandRepository;
        private IUserRepository _userRepository;
        private Serilog.ILogger _logger;
        private BandSearchMemoryCache _memoryCache;
        private IRepository<InstrumentLevel> _instrumentLevelrepository;

        public BandService(
            Serilog.ILogger logger,
            IBandRepository bandRepository,
            IUserRepository userRepository,
            BandSearchMemoryCache memoryCache, 
            IRepository<InstrumentLevel> instrumentLevelrepository)
        {
            _bandRepository = bandRepository;
            _userRepository = userRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _instrumentLevelrepository = instrumentLevelrepository;
    }

        private void SetCacheValue(string cacheKey, object cacheValue) 
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetSlidingExpiration(TimeSpan.FromSeconds(_cacheSlidingExpirationTime))
                   .SetAbsoluteExpiration(TimeSpan.FromSeconds(_cacheAbsoluteExpirationTime))
                   .SetSize(_cacheEntrySize);

            _memoryCache.Cache.Set(cacheKey, cacheValue, cacheEntryOptions);
        }

        private async Task<User> GetUserOrThrowAsync(int userId, CancellationToken cancellationToken)
        {
            await s_semaphore.WaitAsync();

            string cacheKey = "user_" + userId;

            if (!_memoryCache.Cache.TryGetValue(cacheKey, out User? cacheValue))
            {
                _logger.Information($"No user with key {cacheKey} in cache");

                cacheValue = await _userRepository.FindAsync(userId, cancellationToken);

                if (cacheValue == null)
                {
                    _logger.Error($"User with id {userId} does not exist");
                    throw new NotFoundException($"user {userId} not found");
                }

                SetCacheValue(cacheKey, cacheValue);

                _logger.Information($"Set user with id {userId} to cache with key {cacheKey}");
            }

            s_semaphore.Release();

            return cacheValue!;
        }

        private void InvalidateCache(Band band) 
        {
            foreach (var member in band.Members)
            {
                _memoryCache.Cache.Remove("user_" + member.Id);
            }

            _memoryCache.Cache.Remove("band_" + band.Id);
        }

        private async Task<Band> GetBandOrThrowAsync(int bandId, CancellationToken cancellationToken)
        {
            await s_semaphore.WaitAsync();

            string cacheKey = "band_" + bandId;

            if (!_memoryCache.Cache.TryGetValue(cacheKey, out Band? cacheValue))
            {
                _logger.Information($"No band with key {cacheKey} in cache");

                cacheValue = await _bandRepository.FindAsync(bandId, cancellationToken);

                if (cacheValue == null)
                {
                    _logger.Error($"band with id {bandId} does not exist");
                    throw new NotFoundException($"band {bandId} not found");
                }

                SetCacheValue(cacheKey, cacheValue);

                _logger.Information($"Set band with id {bandId} to cache with key {cacheKey}");
            }

            s_semaphore.Release();

            return cacheValue!;
        }

        public async Task<Band> CreateBandAsync(string bandName, int userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            _logger.Information($"User with id {userId} is found");

            var band = new Band() { Name = bandName, OwnerId = userId };
            band.Members.Add(user);

            _logger.Information($"Owner with id {userId} is set and added to band with id {band.Id}");

            try 
            {
                var createdBand = await _bandRepository.InsertAsync(band, cancellationToken);
                _logger.Information($"New band created with id {createdBand.Id}");

                InvalidateCache(band);

                return createdBand;
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with creation of band with name {bandName} and creator with id = {userId}");
                throw new DALException("Error occured during band creation and saving it to the database");
            }
        }

        public async Task AddUserToBandAsync(int userId, int bandId, CancellationToken cancellationToken)
        {
            var band = await GetBandOrThrowAsync(bandId, cancellationToken);

            _logger.Information($"Band with id {bandId} is found");

            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            _logger.Information($"User with id {userId} is found");

            band.Members.Add(user);

            try 
            {
                await _bandRepository.UpdateAsync(band, cancellationToken);
                _logger.Information($"User with id {userId} is added to band with id {bandId}");

                InvalidateCache(band);
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with adding user with id {userId} to the band with id = {bandId}");
                throw new DALException("Error occured during adding new member to the band and updating the database");
            }
        }

        public async Task RemoveUserFromBandAsync(int userId, int bandId, CancellationToken cancellationToken)
        {
            var band = await GetBandOrThrowAsync(bandId, cancellationToken);

            _logger.Information($"Band with id {bandId} is found");

            if (userId == band.OwnerId)
            {
                _logger.Error($"User with id {userId} is owner of band with id {bandId} and can not be removed");
                throw new InvalidOperationException($"User with id = {userId} is owner can not be removed");
            }

            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            _logger.Information($"User with id = {userId} is found and it is not an owner");

            band.Members.Remove(user);

            try 
            {
                await _bandRepository.UpdateAsync(band, cancellationToken);
                _logger.Information($"User with id {userId} is removed from band with id {bandId}");

                InvalidateCache(band);
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with removing user with id {userId} from the band with id {bandId}");
                throw new DALException("Error occured during removing member from the band and saving it to the database");
            }
        }

        public async Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(int bandId, CancellationToken cancellationToken)
        {
            var band = await GetBandOrThrowAsync(bandId, cancellationToken);

            _logger.Information($"Band with id {bandId} found");

            return band.OpenPositions.ToList();
        }

        public async Task<BandOpenPosition> CreateOpenPositionAsync(BandOpenPosition bandOpenPosition, CancellationToken cancellationToken)
        {
            try
            {
                var band = await GetBandOrThrowAsync(bandOpenPosition.BandId, cancellationToken);

                bandOpenPosition.InstrumentLevel.BandOpenPositionId = bandOpenPosition.Id;
                bandOpenPosition.InstrumentLevel.UserId = null;

                band.OpenPositions.Add(bandOpenPosition);

                await _bandRepository.UpdateAsync(band, cancellationToken);

                InvalidateCache(band);

                return bandOpenPosition;
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with creating new band open position with id = {bandOpenPosition.Id}");
                throw new DALException("Error occured during new band open position creation and saving it to the database");
            }
        }

        public async Task DeleteOpenPositionAsync(int bandId, int bandOpenPositionId, CancellationToken cancellationToken)
        {
            try 
            {
                var band = await GetBandOrThrowAsync(bandId, cancellationToken);

                var bandOpenPosition = band.OpenPositions.FirstOrDefault(p => p.Id == bandOpenPositionId);

                if (bandOpenPosition == null)
                {
                    _logger.Error($"Open position with id {bandOpenPositionId} does not exist");
                    throw new NotFoundException($"Open position with id {bandOpenPositionId} does not exist");
                }

                band.OpenPositions.Remove(bandOpenPosition);

                if (bandOpenPosition.InstrumentLevel == null)
                {
                    await _bandRepository.UpdateAsync(band, cancellationToken);
                }
                else
                {
                    await _instrumentLevelrepository.DeleteAsync(bandOpenPosition.InstrumentLevel, cancellationToken);
                }

                InvalidateCache(band);

                _logger.Error($"Open position with id {bandOpenPositionId} was deleted");
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with deleting band open position with id = {bandOpenPositionId}");
                throw new DALException("Error occured during band open position removing and updating the database");
            }
        }

        public async Task DeleteBand(int bandId, CancellationToken cancellationToken)
        {
            var band = await GetBandOrThrowAsync(bandId, cancellationToken);

            _logger.Information($"Band with id {bandId} found");

            InvalidateCache(band);

            foreach (var position in band.OpenPositions)
            { 
                var instrumentLevel = position.InstrumentLevel;
                await _instrumentLevelrepository.DeleteAsync(instrumentLevel, cancellationToken);   
            }

            band.OpenPositions.Clear();

            _logger.Information($"Open positions are removed from band with id {bandId}");

            band.Members.Clear();

            _logger.Information($"Members are removed from band with id {bandId}");

            await _bandRepository.DeleteAsync(bandId, cancellationToken);

            _logger.Information($"Band with id {bandId} was deleted");
        }
    }
}
