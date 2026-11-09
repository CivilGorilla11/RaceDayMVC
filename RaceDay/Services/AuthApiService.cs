using System.Net;
using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Account;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

// THE KITCHEN for login and registration.
// Registered WITHOUT JwtHandler in Program.cs, because a user who is logging in has no token yet.
public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _http;   // already knows the API base URL

    public AuthApiService(HttpClient http)
    {
        _http = http;
    }

    // ---------- LOGIN ----------
    public async Task<ApiResult<LoginResponse>> LoginAsync(LoginViewModel model)
    {
        try
        {
            // Send only what the API expects: { email, password }
            var body = new { model.Email, model.Password };
            var response = await _http.PostAsJsonAsync("api/auth/login", body);

            // IMPORTANT: on the login call, 401 means "wrong email or password", NOT "session expired".
            // That is why this method does not use the shared message for 401.
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return ApiResult<LoginResponse>.Fail("Invalid email or password.", 401);

            if (!response.IsSuccessStatusCode)
                return ApiResult<LoginResponse>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            // Turn the JSON ({ "token": "...", "role": "..." }) into a LoginResponse object
            var login = await response.Content.ReadFromJsonAsync<LoginResponse>();

            // Never trust the API blindly: a login without a token or role is useless to us
            if (login == null || string.IsNullOrWhiteSpace(login.Token) || string.IsNullOrWhiteSpace(login.Role))
                return ApiResult<LoginResponse>.Fail("The API did not return a valid login response.", 500);

            return ApiResult<LoginResponse>.Ok(login, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            // The API is not running or the base URL is wrong
            return ApiResult<LoginResponse>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // ---------- REGISTER ----------
    // The two public methods only differ by URL, so they share one private helper below.
    public Task<ApiResult<bool>> RegisterParticipantAsync(RegisterViewModel model) =>
        RegisterAsync("api/auth/register/participant", model);

    // CHECK in Swagger: if your organiser endpoint has a different path, change only this string.
    public Task<ApiResult<bool>> RegisterOrganiserAsync(RegisterViewModel model) =>
        RegisterAsync("api/auth/register/organiser", model);

    private async Task<ApiResult<bool>> RegisterAsync(string url, RegisterViewModel model)
    {
        try
        {
            // ConfirmPassword is left out on purpose: it exists for the form, the API does not expect it.
            var body = new { model.Name, model.Email, model.Password };
            var response = await _http.PostAsJsonAsync(url, body);

            // 400 or 409 usually means the email is already registered or the data was rejected
            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict)
                return ApiResult<bool>.Fail("We could not create the account. That email may already be registered.", (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return ApiResult<bool>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            // The API returns an id and a token here, but we do not use them: the user logs in next.
            return ApiResult<bool>.Ok(true, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }
}
