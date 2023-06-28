using BandSearch.Models;
using BandSearch.Web.DTOs;
using BandSearch.Web.Models;

namespace BandSearch.Web.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAndGenerateTokenAsync(UserCredentials userCredentials, CancellationToken cancellationToken);
        Task<User> RegisterAsync(User newUser, CancellationToken cancellationToken);
    }
}
