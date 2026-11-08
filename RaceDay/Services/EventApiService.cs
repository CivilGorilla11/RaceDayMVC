using System.Net.Http.Json;
using RaceDay.Models;
using RaceDay.Models.Events;
using RaceDay.Services.Interfaces;

namespace RaceDay.Services;

// THE KITCHEN for events. ": IEventApiService" promises every method on the menu (the interface).
// Every method follows the same four steps:
//   1. try the API call
//   2. not a success status code -> return ApiResult.Fail(friendly message)
//   3. success -> turn the JSON into our model and return ApiResult.Ok(data)
//   4. catch HttpRequestException = the API could not be reached at all
public class EventApiService : IEventApiService
{
    // Handed to us by ASP.NET. It already knows the base URL (and the JWT, once JwtHandler exists).
    private readonly HttpClient _http;

    public EventApiService(HttpClient http)
    {
        _http = http;
    }

    // GET /api/events?search=&location=&date=
    public async Task<ApiResult<List<EventViewModel>>> GetEventsAsync(EventFilterViewModel? filter = null)
    {
        try
        {

            // Build the query string only from filters the user actually filled in.
            var parts = new List<string>();
            var search = filter?.Search;
            var location = filter?.Location;

            if (!string.IsNullOrWhiteSpace(search))
                parts.Add($"search={Uri.EscapeDataString(search)}");    // makes "10 km" URL-safe
            if (!string.IsNullOrWhiteSpace(location))
                parts.Add($"location={Uri.EscapeDataString(location)}");
            if (filter?.Date != null)
                parts.Add($"date={filter.Date.Value:yyyy-MM-dd}");

            var url = "api/Event" + (parts.Count > 0 ? "?" + string.Join("&", parts) : "");

          

            var response = await _http.GetAsync(url);
            Console.WriteLine($"EVENTS CALL: {(int)response.StatusCode} {response.RequestMessage?.RequestUri}");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<EventViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            // Converts the JSON text into C# objects. "?? new()" = empty list instead of null.
            var events = await response.Content.ReadFromJsonAsync<List<EventViewModel>>() ?? new();
            return ApiResult<List<EventViewModel>>.Ok(events, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<EventViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // GET /api/events/{id}
    public async Task<ApiResult<EventViewModel>> GetEventAsync(int eventId)
    {
        try
        {
            var response = await _http.GetAsync($"api/Event/{eventId}");
            if (!response.IsSuccessStatusCode)
                return ApiResult<EventViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var ev = await response.Content.ReadFromJsonAsync<EventViewModel>();
            if (ev == null)
                return ApiResult<EventViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<EventViewModel>.Ok(ev, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<EventViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // GET /api/events/organiser/me  (the API reads who the organiser is from the JWT token)
    public async Task<ApiResult<List<EventViewModel>>> GetMyEventsAsync()
    {
        try
        {
            var response = await _http.GetAsync("api/Event/organiser/me");
            if (!response.IsSuccessStatusCode)
                return ApiResult<List<EventViewModel>>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var events = await response.Content.ReadFromJsonAsync<List<EventViewModel>>() ?? new();
            return ApiResult<List<EventViewModel>>.Ok(events, (int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResult<List<EventViewModel>>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // POST /api/events
    public async Task<ApiResult<EventViewModel>> CreateEventAsync(EventFormViewModel model)
    {
        try
        {
            // Send ONLY the fields the API expects (an anonymous object). OrganiserID comes from the token.
            var body = new { model.EventName, model.EventDate, model.Address, model.Description };

            var response = await _http.PostAsJsonAsync("api/Event", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<EventViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<EventViewModel>();
            if (created == null)
                return ApiResult<EventViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<EventViewModel>.Ok(created, (int)response.StatusCode); // 201 Created
        }
        catch (HttpRequestException)
        {
            return ApiResult<EventViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // PUT /api/events/{id}
    public async Task<ApiResult<EventViewModel>> UpdateEventAsync(int eventId, EventFormViewModel model)
    {
        try
        {
            var body = new { model.EventName, model.EventDate, model.Address, model.Description };

            var response = await _http.PutAsJsonAsync($"api/Event/{eventId}", body);
            if (!response.IsSuccessStatusCode)
                return ApiResult<EventViewModel>.Fail(ApiErrorMessages.For(response.StatusCode), (int)response.StatusCode);

            var updated = await response.Content.ReadFromJsonAsync<EventViewModel>();
            if (updated == null)
                return ApiResult<EventViewModel>.Fail("The API returned an empty response.", 500);

            return ApiResult<EventViewModel>.Ok(updated, (int)response.StatusCode); // 200 OK
        }
        catch (HttpRequestException)
        {
            return ApiResult<EventViewModel>.Fail(ApiErrorMessages.Unreachable, 503);
        }
    }

    // DELETE /api/events/{id}
    public async Task<ApiResult<bool>> DeleteEventAsync(int eventId)
    {
        try
        {
            var response = await _http.DeleteAsync($"api/Event/{eventId}");

            // 204 No Content has no body to read, so success is simply "true".
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
