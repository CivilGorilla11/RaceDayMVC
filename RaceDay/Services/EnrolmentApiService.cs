using System.Net;
using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Enrolments;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

public class EnrolmentApiService : IEnrolmentApiService
{
    private readonly HttpClient _http;

    public EnrolmentApiService(HttpClient http)
    {
        _http = http;
    }

    // ORGANISER: GET /api/Enrollments/api/events/{eventId}/enrollments
    public async Task<ApiResult<List<EnrolmentViewModel>>> GetForEventAsync(int eventId)
    {
        try
        {
            var response = await _http.GetAsync($"api/Enrollments/api/events/{eventId}/enrollments");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<EnrolmentViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var enrolments = await response.Content.ReadFromJsonAsync<List<EnrolmentViewModel>>() ?? new();
            return ApiResult<List<EnrolmentViewModel>>.Ok(enrolments, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<EnrolmentViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PARTICIPANT: POST /api/Enrollments/enrollments   body: { categoryId }
    // The participant is taken from the JWT by the API, so we only send the category.
    public async Task<ApiResult<EnrolmentViewModel>> EnrolAsync(int categoryId)
    {
        try
        {
            var body = new { CategoryId = categoryId };
            var response = await _http.PostAsJsonAsync("api/Enrollments/enrollments", body);

            // 400 / 409 usually means "already enrolled" or "category is full"
            if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Conflict)
                return ApiResult<EnrolmentViewModel>.Fail(
                    "We could not enrol you. You may already be enrolled in this category, or it may be full.",
                    (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return ApiResult<EnrolmentViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<EnrolmentViewModel>();
            if (created == null)
                return ApiResult<EnrolmentViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<EnrolmentViewModel>.Ok(created, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<EnrolmentViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PARTICIPANT: GET /api/Enrollments/enrollments
    // CHECK in Swagger that this returns the LOGGED-IN user's enrolments, not everyone's.
    public async Task<ApiResult<List<EnrolmentViewModel>>> GetMineAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/Enrollments/enrollments");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<EnrolmentViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var enrolments = await response.Content.ReadFromJsonAsync<List<EnrolmentViewModel>>() ?? new();
            return ApiResult<List<EnrolmentViewModel>>.Ok(enrolments, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<EnrolmentViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PARTICIPANT: DELETE /api/Enrollments/api/enrollments/{enrollmentId}
    public async Task<ApiResult<bool>> WithdrawAsync(int enrolmentId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/Enrollments/api/enrollments/{enrolmentId}");
            if (!response.IsSuccessStatusCode)
                return ApiResult<bool>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            return ApiResult<bool>.Ok(true, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }
}

