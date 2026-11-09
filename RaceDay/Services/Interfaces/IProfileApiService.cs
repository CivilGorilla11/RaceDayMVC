using RaceDay.Models;
using RaceDay.Models.Profile;

namespace RaceDay.Services.Interfaces;

public interface IProfileApiService
{
    Task<ApiResult<ProfileViewModel>> GetMeAsync();                         // GET /api/Participant/me
    Task<ApiResult<ProfileViewModel>> UpdateMeAsync(ProfileViewModel model); // PUT /api/Participant/me
}
