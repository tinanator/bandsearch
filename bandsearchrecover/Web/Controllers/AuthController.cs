using BandSearch.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BandSearch.Web.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using BandSearch.Web.DTOs;
using BandSearch.Web.Mappers;

namespace BandSearch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private IValidator<UserRegisterDataDTO> _userRegisterDataValidator;
        private IValidator<UserCredentials> _userCredentialsValidator;
        private UserRegisterDataDTOMapper _userRegisterDataDTOMapper;
        private Serilog.ILogger _logger;
        private UserMapper _userMapper;

        public AuthController(IAuthService loginService,
            IValidator<UserRegisterDataDTO> userRegisterDataValidator,
            UserRegisterDataDTOMapper userRegisterDataDTOMapper, 
            IValidator<UserCredentials> userCredentialsValidator,
            Serilog.ILogger logger, 
            UserMapper userMapper)
        {
            _authService = loginService;
            _userRegisterDataValidator = userRegisterDataValidator;
            _userRegisterDataDTOMapper = userRegisterDataDTOMapper;
            _userCredentialsValidator = userCredentialsValidator;
            _logger = logger;
            _userMapper = userMapper;
        }

        [HttpPost]
        public async Task<string> Login([FromBody] UserCredentials userCredentials, CancellationToken cancellationToken)
        {
            _logger.Information($"Login user with credentials {userCredentials.Email}");

            _userCredentialsValidator.ValidateAndThrow(userCredentials);

            _logger.Information($"User credentials are valid");

            return await _authService.AuthenticateAndGenerateTokenAsync(userCredentials, cancellationToken);
        }

        [HttpPost("register")]
        public async Task<UserDTO> Register([FromBody] UserRegisterDataDTO userRegisterDataDTO, CancellationToken cancellationToken)
        {
            _logger.Information($"Register new user with email {userRegisterDataDTO.Email}" +
                $" name {userRegisterDataDTO.Name} surname {userRegisterDataDTO.Surname}");

            _userRegisterDataValidator.ValidateAndThrow(userRegisterDataDTO);

            _logger.Information($"Register data are valid");

            var user = _userRegisterDataDTOMapper.UserRegisterDataToUser(userRegisterDataDTO);
            var newUser = await _authService.RegisterAsync(user, cancellationToken);

            _logger.Information($"User with email {newUser.Email} is registered");

            return _userMapper.UserToUserDto(newUser);
        }
    }
}
