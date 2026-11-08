using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Results;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

// THE KITCHEN for race results.
// For the participant "My Results" page, add this to BOTH this class and IResultApiService later:
//   GetMineAsync()  -> GET api/results/me
public class ResultApiService : IResultApiService
{
    private readonly HttpClient _http;

    public ResultApiService(HttpClient http)
    {
        _http = http;
    }

    // GET /api/events/{eventId}/results  (the event's leaderboard)
    public async Task<ApiResult<List<ResultViewModel>>> GetForEventAsync(int eventId)
    {
        try
        {
            var response = await _http.GetAsync($"api/Results/api/events/{eventId}/results");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<ResultViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<List<ResultViewModel>>() ?? new();
            return ApiResult<List<ResultViewModel>>.Ok(results, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<ResultViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // POST /api/results
    public async Task<ApiResult<ResultViewModel>> CreateAsync(ResultFormViewModel model)
    {
        try
        {
            // Four fields only. model.EventId and model.ResultId are MVC-only and must NOT be sent.
            var body = new { model.EnrolmentId, model.FinishTime, model.Position, model.Race };

            var response = await _http.PostAsJsonAsync("api/Results/api/results", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ResultViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ResultViewModel>();
            if (created == null)
                return ApiResult<ResultViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<ResultViewModel>.Ok(created, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<ResultViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PUT /api/results/{resultId}
    public async Task<ApiResult<ResultViewModel>> UpdateAsync(int resultId, ResultFormViewModel model)
    {
        try
        {
            // The update body in your plan has no enrolmentId: a result stays attached to its enrolment.
            var body = new { model.FinishTime, model.Position, model.Race };

            var response = await _http.PutAsJsonAsync($"api/Results/api/{resultId}", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<ResultViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<ResultViewModel>();
            if (updated == null)
                return ApiResult<ResultViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<ResultViewModel>.Ok(updated, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<ResultViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // DELETE /api/results/{resultId}
    public async Task<ApiResult<bool>> DeleteAsync(int resultId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/Results/api/results/{resultId}");
            if (!response.IsSuccessStatusCode)
                return ApiResult<bool>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            return ApiResult<bool>.Ok(true, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<bool>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PARTICIPANT: GET /api/Results/api/results/me  (the logged-in participant's own results)
    public async Task<ApiResult<List<ResultViewModel>>> GetMineAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/Results/api/results/me");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<ResultViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var results = await response.Content.ReadFromJsonAsync<List<ResultViewModel>>() ?? new();
            return ApiResult<List<ResultViewModel>>.Ok(results, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<ResultViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }
}