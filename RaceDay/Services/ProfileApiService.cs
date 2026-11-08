using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Profile;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

public class ProfileApiService : IProfileApiService
{
    private readonly HttpClient _http;

    public ProfileApiService(HttpClient http)
    {
        _http = http;
    }

    // GET /api/Participant/me
    public async Task<ApiResult<ProfileViewModel>> GetMeAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/Participant/me");
            if (!response.IsSuccessStatusCode)
                return ApiResult<ProfileViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var profile = await response.Content.ReadFromJsonAsync<ProfileViewModel>();
            if (profile == null)
                return ApiResult<ProfileViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<ProfileViewModel>.Ok(profile, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<ProfileViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PUT /api/Participant/me
    public async Task<ApiResult<ProfileViewModel>> UpdateMeAsync(ProfileViewModel model)
    {
        try
        {
            // Build the body field by field so a blank password is NOT sent (blank = "keep my current one").
            var body = new Dictionary<string, object?>
            {
                ["name"] = model.Name,
                ["email"] = model.Email
            };
            if (!string.IsNullOrWhiteSpace(model.Password))
                body["password"] = model.Password;

            var response = await _http.PutAsJsonAsync("api/Participant/me", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ProfileViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<ProfileViewModel>();
            return ApiResult<ProfileViewModel>.Ok(updated ?? model, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<ProfileViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }
}