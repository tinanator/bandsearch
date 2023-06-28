using System.Collections.Immutable;
using BandSearch.Web.DTOs;
using BandSearch.Web.Mappers;
using BandSearch.Web.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BandSearch.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BandController : ControllerBase
    {
        private IBandService _bandService;
        private IValidator<string> _validator;
        private BandMapper _bandMapper;
        private BandOpenPositionMapper _bandOpenPositionMapper;
        private Serilog.ILogger _logger;
        private IValidator<BandOpenPositionDTO> _bandOpenPositionValidator;

        public BandController(IBandService bandService,
            IValidator<string> validator,
            BandMapper bandMapper,
            BandOpenPositionMapper bandOpenPositionMapper,
            Serilog.ILogger logger,
            IValidator<BandOpenPositionDTO> bandOpenPositionValidator)
        {
            _bandService = bandService;
            _validator = validator;
            _bandMapper = bandMapper;
            _bandOpenPositionMapper = bandOpenPositionMapper;
            _logger = logger;
            _bandOpenPositionValidator = bandOpenPositionValidator;
        }

        [HttpPost("{bandName}/{userId}")]
        public async Task<BandDTO> Create(int userId, string bandName, CancellationToken cancellationToken)
        {
            _logger.Information($"Create band with name {bandName} and owner with id {userId}");
            _validator.ValidateAndThrow(bandName);
            _logger.Information($"Band data are valid");

            var createdBand = await _bandService.CreateBandAsync(bandName, userId, cancellationToken);

            _logger.Information($"New band is created with id {createdBand.Id} " +
                $"name {createdBand.Name} " +
                $"ownerId {createdBand.OwnerId}");

            return _bandMapper.BandToBandDTO(createdBand); ;
        }

        [HttpDelete("{bandId}")]
        public async Task Delete(int bandId, CancellationToken cancellationToken)
        {
            _logger.Information($"Delete band with id {bandId}");
            await _bandService.DeleteBand(bandId, cancellationToken);
        }

        [HttpPut("{bandId}/invite-member/{userId}")]
        public async Task InviteMember(int userId, int bandId, CancellationToken cancellationToken)
        {
            _logger.Information($"Invite user with id {userId} to the band with id {bandId}");
            await _bandService.AddUserToBandAsync(userId, bandId, cancellationToken);
        }

        [HttpGet("open-positions/{bandId}")]
        public async Task<IReadOnlyCollection<BandOpenPositionDTO>> GetOpenPositions(int bandId, CancellationToken cancellationToken)
        {
            _logger.Information($"Get band open positions by the band with id {bandId}");

            var bandOpenPositions = await _bandService.GetOpenPositionsAsync(bandId, cancellationToken);
            var bandOpenPositionsDTO = bandOpenPositions
                .Select(openPosition => _bandOpenPositionMapper.OpenPositionToOpenPositionDTO(openPosition))
                .ToImmutableList();

            return bandOpenPositionsDTO;
        }

        [HttpDelete("{bandId}/member/{userId}")]
        public async Task RemoveUser(int userId, int bandId, CancellationToken cancellationToken)
        {
            _logger.Information($"Remove user with id {userId} from band with id {bandId}");
            await _bandService.RemoveUserFromBandAsync(userId, bandId, cancellationToken);
        }

        [HttpPost("open-position")]
        public async Task<BandOpenPositionDTO> CreateOpenPosition([FromBody] BandOpenPositionDTO bandOpenPositionDTO, CancellationToken cancellationToken)
        {
            _logger.Information($"Create new open position");

            _bandOpenPositionValidator.ValidateAndThrow(bandOpenPositionDTO);

            var bandOpenPosition = _bandOpenPositionMapper.OpenPositionDTOToOpenPosition(bandOpenPositionDTO);
            var createdBandOpenPosition = await _bandService.CreateOpenPositionAsync(bandOpenPosition, cancellationToken);

            _logger.Information($"New position is created with id {createdBandOpenPosition.Id}");

            return _bandOpenPositionMapper.OpenPositionToOpenPositionDTO(createdBandOpenPosition);
        }

        [HttpDelete("{bandId}/open-position/{openPositionId}")]
        public async Task DeleteOpenPosition(int bandId, int openPositionId, CancellationToken cancellationToken)
        {
            _logger.Information($"Delete open position with id {openPositionId}");
            await _bandService.DeleteOpenPositionAsync(bandId, openPositionId, cancellationToken);
        }
    }
}
