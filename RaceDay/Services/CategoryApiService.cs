using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Categories;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

// THE KITCHEN for categories (the "10K Run", "Half Marathon" options inside an event).
// Same four-step pattern as EventApiService.
public class CategoryApiService : ICategoryApiService
{
    private readonly HttpClient _http;

    public CategoryApiService(HttpClient http)
    {
        _http = http;
    }

    // GET /api/events/{eventId}/categories
    public async Task<ApiResult<List<CategoryViewModel>>> GetForEventAsync(int eventId)
    {
        try
        {
            var response = await _http.GetAsync($"api/events/{eventId}/Category");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<CategoryViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var categories = await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>() ?? new();
            return ApiResult<List<CategoryViewModel>>.Ok(categories, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<CategoryViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // POST /api/events/{eventId}/categories
    public async Task<ApiResult<CategoryViewModel>> CreateAsync(int eventId, CategoryFormViewModel model)
    {
        try
        {
            // The event id travels in the URL, so it is NOT part of the body.
            var body = new { model.CategoryName, model.Distance, model.EntryFee, model.MaxParticipants };

            var response = await _http.PostAsJsonAsync($"api/events/{eventId}/Category", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CategoryViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<CategoryViewModel>();
            if (created == null)
                return ApiResult<CategoryViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<CategoryViewModel>.Ok(created, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<CategoryViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PUT /api/categories/{categoryId}
    public async Task<ApiResult<CategoryViewModel>> UpdateAsync(int categoryId, CategoryFormViewModel model)
    {
        try
        {
            var body = new { model.CategoryName, model.Distance, model.EntryFee, model.MaxParticipants };

            var response = await _http.PutAsJsonAsync($"api/Category/{categoryId}", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<CategoryViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<CategoryViewModel>();
            if (updated == null)
                return ApiResult<CategoryViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<CategoryViewModel>.Ok(updated, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<CategoryViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // DELETE /api/categories/{categoryId}
    public async Task<ApiResult<bool>> DeleteAsync(int categoryId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/Category/{categoryId}");
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