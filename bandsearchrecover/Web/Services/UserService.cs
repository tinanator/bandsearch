using BandSearch.Web.BusinessModels;
using BandSearch.Web.Database;
using BandSearch.Models;
using BandSearch.Web.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using BandSearch.Web.Models;
using BandSearch.Database;

namespace BandSearch.Web.Services
{
    public class UserService : IUserService
    {
        private static readonly SemaphoreSlim s_semaphore = new SemaphoreSlim(1, 1);
        private readonly int _cacheSlidingExpirationTime = 10;
        private readonly int _cacheAbsoluteExpirationTime = 20;
        private readonly int _cacheEntrySize = 1;

        private IUserRepository _userRepository;
        private Serilog.ILogger _logger;
        private BandSearchMemoryCache _memoryCache;
        private IBandService _bandService;
        private IRepository<InstrumentLevel> _instrumentLevelRepository;

        public UserService(
            IUserRepository userRepository,
            Serilog.ILogger logger,
            BandSearchMemoryCache memoryCache,
            IBandService bandService,
            IRepository<InstrumentLevel> instrumentLevelRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _memoryCache = memoryCache;
            _bandService = bandService;
            _instrumentLevelRepository = instrumentLevelRepository;
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
            else 
            {
                _logger.Information($"User with key {cacheKey} was found in the cache");
            }

            s_semaphore.Release();
            return cacheValue!;
        }

        private void InvalidateCache(User user) 
        {
            _memoryCache.Cache.Remove("user_" + user.Id);

            foreach (var band in user.Bands)
            {
                foreach (var position in band.OpenPositions)
                {
                    _memoryCache.Cache.Remove("position_" + position.Id);
                }

                _memoryCache.Cache.Remove("band_" + band.Id);
            }
        }

        public async Task<UserDetails> GetUserDetailsAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            _logger.Information($"User with id {userId} was found");

            var bandOpenPositions = await GetUserBandOpenPositionsAsync(userId, cancellationToken);
            _logger.Information($"Band open positions of the user with id {userId} are retrieved");

            var bands = await GetBandViewsAsync(userId, cancellationToken);
            _logger.Information($"Bands of the user with id {userId} are retrieved");

            var userDetails = FormUserDetails(user, bandOpenPositions, bands);

            return userDetails;
        }

        private UserDetails FormUserDetails(User user,
            IReadOnlyCollection<BandOpenPosition> bandOpenPositions,
            IReadOnlyCollection<BandView> bands) 
        {
            var userDetails = new UserDetails()
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Age = user.Age,
                Gender = user.Gender,
                Country = user.Country,
                City = user.City,
                PhotoUrl = user.PhotoUrl,
                About = user.About,
                IsLookingForBand = user.IsLookingForBand,
                InstrumentsLevel = user.InstrumentsLevel,
                BandOpenPositionCriteriaInfo = user.BandOpenPositionCriteriaInfo,
                BandOpenPositions = bandOpenPositions.ToList(),
                IsBandLookingFor = bandOpenPositions.Any(),
                Bands = bands.ToList()
            };

            userDetails.IsBandLookingFor = userDetails.BandOpenPositions.Any();

            return userDetails;
        }

        public async Task<IReadOnlyCollection<BandOpenPosition>> GetUserBandOpenPositionsAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            var userBands = user.Bands.ToList();
            _logger.Information($"Bands of the user {userId} are retrieved");

            return user.Bands.SelectMany(b => b.OpenPositions).ToList();
        }

        public async Task<IReadOnlyCollection<BandView>> GetBandViewsAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            var userBands = user.Bands.ToList();

            _logger.Information($"Bands of the user {userId} are retrieved");

            var bandsViews = new List<BandView>();

            foreach (var band in userBands)
            {
                var bandView = new BandView()
                {
                    BandId = band.Id,
                    BandName = band.Name
                };

                bandView.Members = band.Members.Select(member => new BandMemberView()
                {
                    Id = member.Id,
                    Name = member.Name,
                    Surname = member.Surname
                }).ToList();

                _logger.Information($"Band view {JsonConvert.SerializeObject(bandView)} is formed");

                bandsViews.Add(bandView);
            }

            _logger.Information($"Band views are formed");

            return bandsViews.AsReadOnly();
        }

        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            try 
            {
                return await _userRepository.InsertAsync(user, cancellationToken);
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with creation a user with email {user.Email}");
                throw new DALException("Error occured during user creation and saving it to the database");
            }
        }

        public async Task DeleteUserAsync([FromQuery] int userId, CancellationToken cancellationToken)
        {
            try 
            {
                var user = await GetUserOrThrowAsync(userId, cancellationToken);

                foreach (var id in user.Bands.Select(b => b.Id).ToList())
                {
                    if (id == userId)
                    {
                        await _bandService.DeleteBand(id, cancellationToken);
                    }
                    else 
                    {
                        await _bandService.RemoveUserFromBandAsync(userId, id, cancellationToken);
                    }
                }

                foreach (var id in user.InstrumentsLevel.Select(i => i.Id).ToList())
                { 
                    await _instrumentLevelRepository.DeleteAsync(id, cancellationToken);
                }

                await _userRepository.DeleteAsync(user, cancellationToken);

                _logger.Information($"User with id {userId} was deleted");
            }
            catch (DbUpdateException)
            {
                _logger.Error($"Error with deleting a user with id {userId}");
                throw new DALException("Error occured during user removing and updating the database");
            }
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            try 
            {
                var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

                InvalidateCache(user);

                _logger.Information($"User with id {user.Id} was updated");
                return updatedUser;
            }
            catch (DbUpdateException )
            {
                _logger.Error($"Error with updating a user with id {user.Id}");
                throw new DALException("Error occured during updating user in the database");
            }
        }

        public async Task<IReadOnlyCollection<UserDetails>> GetListAsync(CancellationToken cancellationToken)
        {
            var list = await _userRepository.GetAllAsync(cancellationToken);
            var userDetailsList = new List<UserDetails>();

            foreach (var user in list)
            {
                var bandOpenPositions = await GetUserBandOpenPositionsAsync(user.Id, cancellationToken);
                var bands = await GetBandViewsAsync(user.Id, cancellationToken);
                var userDEtails = FormUserDetails(user, bandOpenPositions, bands);
                userDetailsList.Add(userDEtails);
            }

            return userDetailsList.AsReadOnly();
        }

        public async Task<User> AddInstrumentLevelAsync(int userId, string instrument, Level level, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);

            var instrumentToLevel = new InstrumentLevel() 
            {
                UserId = userId,
                Instrument = instrument,
                Level = level,
            };

            user.InstrumentsLevel.Add(instrumentToLevel);

            InvalidateCache(user);

            return await _userRepository.UpdateAsync(user, cancellationToken);
        }

        public async Task DeleteInstrumentLevelAsync(int userId, int instrumentLevelId, CancellationToken cancellationToken)
        {
            var user = await GetUserOrThrowAsync(userId, cancellationToken);
            var instrumentToLevel = user.InstrumentsLevel.FirstOrDefault(i => i.Id == instrumentLevelId);

            if (instrumentToLevel == null)
            {
                _logger.Error($"Instrument with id {instrumentLevelId} does not exist");
                throw new NotFoundException($"Instrument with id {instrumentLevelId} does not exist");
            }

            await _instrumentLevelRepository.DeleteAsync(instrumentToLevel, cancellationToken);

            InvalidateCache(user);

            await _userRepository.UpdateAsync(user, cancellationToken);
        }
    }
}
