using BandSearch.Web.Services;
using Microsoft.AspNetCore.Mvc;
using BandSearch.Web.BusinessModels;
using FluentValidation;
using BandSearch.Web.DTOs;
using BandSearch.Web.Mappers;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Immutable;

namespace BandSearch.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IValidator<UpdateUserDTO> _validator;
        private UserMapper _userMapper;
        private UserDetailsMapper _userDetailsMapper;
        private Serilog.ILogger _logger;
        private IValidator<InstrumentLevelDTO> _instrumentLevelValidator;

        public UserController(IUserService userService,
            IValidator<UpdateUserDTO> validator,
            UserMapper userMapper,
            UserDetailsMapper userDetailsMapper,
            Serilog.ILogger logger, 
            IValidator<InstrumentLevelDTO> instrumentLevelValidator)
        {
            _userService = userService;
            _validator = validator;
            _userMapper = userMapper;
            _userDetailsMapper = userDetailsMapper;
            _logger = logger;
            _instrumentLevelValidator = instrumentLevelValidator;
        }

        [HttpGet("{userId}")]
        public async Task<UserDetailsDTO> Get(int userId, CancellationToken cancellationToken)
        {
            _logger.Information("Get user with id = {userId}", userId);
            var userDetails = await _userService.GetUserDetailsAsync(userId, cancellationToken);
            var userDetailsDTO = _userDetailsMapper.UserDetailsToUserDetailtsDto(userDetails);

            return userDetailsDTO;
        }

        [HttpGet]
        public async Task<IReadOnlyCollection<UserDetailsDTO>> GetList(CancellationToken cancellationToken)
        {
            _logger.Information("Get all users");
            var userDetailsList = await _userService.GetListAsync(cancellationToken);
            var userDetailsDTOList = userDetailsList
                .Select(u => _userDetailsMapper.UserDetailsToUserDetailtsDto(u))
                .ToImmutableList();

            return userDetailsDTOList;
        }

        [HttpGet("bands/{userId}")]
        public async Task<IReadOnlyCollection<BandView>> GetBands(int userId, CancellationToken cancellationToken)
        {
            _logger.Information($"get bands by user with id = {userId}");
            return await _userService.GetBandViewsAsync(userId, cancellationToken);
        }

        [HttpDelete("{userId}")]
        public async Task Delete(int userId, CancellationToken cancellationToken)
        {
            _logger.Information($"Delete user with id = {userId}");
            await _userService.DeleteUserAsync(userId, cancellationToken);
        }

        [HttpPut]
        public async Task<UserDTO> Update([FromBody] UpdateUserDTO updateUserDTO, CancellationToken cancellationToken)
        {
            _logger.Information("Update user {@userDTO}", updateUserDTO);
            _validator.ValidateAndThrow(updateUserDTO);
            _logger.Information($"Input data are valid");

            var user = _userMapper.UpdateUserDTOToUser(updateUserDTO);
            var updatedUser = await _userService.UpdateUserAsync(user, cancellationToken);

            _logger.Information($"User with id {updatedUser.Id} is updated");

            return _userMapper.UserToUserDto(updatedUser);
        }

        [HttpPost("{userId}/instrument-level")]
        public async Task AddInstrumentLevel(int userId, [FromBody] InstrumentLevelDTO instrumentToLevelDTO, CancellationToken cancellationToken) 
        {
            _instrumentLevelValidator.ValidateAndThrow(instrumentToLevelDTO);
            await _userService.AddInstrumentLevelAsync(userId, instrumentToLevelDTO.Instrument, instrumentToLevelDTO.Level, cancellationToken);
        }

        [HttpDelete("{userId}/instrument-level/{instrumentLevelId}")]
        public async Task DeleteInstrumentToLevel(int userId, int instrumentLevelId, CancellationToken cancellationToken) 
        {
            await _userService.DeleteInstrumentLevelAsync(userId, instrumentLevelId, cancellationToken);
        }
    }
}
