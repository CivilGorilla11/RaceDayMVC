using RaceDay.Models;
using RaceDay.Models.Account;

namespace RaceDay.Services.Interfaces
{
    public interface IAuthApiService
    {
        // POST /api/auth/login, returns the JWT and the role ("Participant" or "Organiser")
        Task<ApiResult<LoginResponse>> LoginAsync(LoginViewModel model); 

        // Both register methods return a simple true/false. After registering, 
        // the user is sent to the login page instead of being logged in automatically.
        Task<ApiResult<bool>> RegisterParticipantAsync(RegisterViewModel model);
        Task<ApiResult<bool>> RegisterOrganiserAsync(RegisterViewModel model);
    }
}
