using BandSearch.Models;
using BandSearch.Web.Database;
using BandSearch.Web.Exceptions;
using BandSearch.Web.Models;
using BandSearch.Web.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace BandSearch.Web.Services
{
    public class AuthService : IAuthService
    {
        private IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private IPasswordHasher<User> _passwordHasher;
        private IOptions<JwtOptions> _options;
        private Serilog.ILogger _logger;

        public AuthService(IUserRepository userRepository,
            IConfiguration config,
            IPasswordHasher<User> passwordHasher,
            IOptions<JwtOptions> options,
            Serilog.ILogger logger)
        {
            _userRepository = userRepository;
            _config = config;
            _passwordHasher = passwordHasher;
            _options = options;
            _logger = logger;
        }

        public async Task<User> RegisterAsync(User newUser, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.FindByEmailAsync(newUser.Email, cancellationToken);

            if (currentUser != null)
            {
                _logger.Error($"User with email {newUser.Email} already exists");
                throw new UserAlreadyExistsException("User already exists");
            }

            _logger.Information($"User with email {newUser.Email} does not exist");
            newUser.Password = _passwordHasher.HashPassword(newUser, newUser.Password);

            return await _userRepository.InsertAsync(newUser, cancellationToken);
        }

        public async Task<string> AuthenticateAndGenerateTokenAsync(UserCredentials userCredentials, CancellationToken cancellationToken)
        {
            var currentUser = await _userRepository.FindByEmailAsync(userCredentials.Email, cancellationToken);

            if (currentUser == null)
            {
                _logger.Error($"User with email {userCredentials.Email} does not exist");
                throw new WrongCredentialsException("Wrong credentials");
            }

            _logger.Information($"User with email {userCredentials.Email} was found");

            var result = _passwordHasher.VerifyHashedPassword(currentUser, currentUser.Password, userCredentials.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.Error($"Password for email {userCredentials.Email} is wrong");
                throw new WrongCredentialsException("Wrong credentials");
            }

            _logger.Information($"Password for email {userCredentials.Email} is correct");

            return GenerateToken(currentUser);
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            _logger.Information($"SecurityKey and credentials for token are generated");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            _logger.Information($"Claims for token are set");

            var token = new JwtSecurityToken(
                _options.Value.Issuer,
                _options.Value.Audience,
                claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_options.Value.ExpirationDate)),
                signingCredentials: credentials);

            _logger.Information($"Token for user with email {user.Email} is generated");

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
