using RaceDay.Models;
using RaceDay.Models.Events;

namespace RaceDay.Services.Interfaces
{
    // THE MENU: lists what the events service can do. The class EventApiService does the real work.
    // Every method returns ApiResult<T> so controllers can check Success instead of catching exceptions
    public interface IEventApiService
    {
        Task<ApiResult<List<EventViewModel>>> GetEventsAsync(EventFilterViewModel? filter = null); // GET /api/events?...
        Task<ApiResult<EventViewModel>> GetEventAsync(int eventId);                                // GET /api/events/{id}
        Task<ApiResult<List<EventViewModel>>> GetMyEventsAsync();                                  // GET /api/events/organiser/me
        Task<ApiResult<EventViewModel>> CreateEventAsync(EventFormViewModel model);                // POST /api/events
        Task<ApiResult<EventViewModel>> UpdateEventAsync(int eventId, EventFormViewModel model);   // PUT /api/events/{id}
        Task<ApiResult<bool>> DeleteEventAsync(int eventId);   // DELETE /api/events/{id}

        Task<ApiResult<EventViewModel>> UploadBannerAsync(int eventId, IFormFile file);   // POST /api/events/{id}/banner
    }
}
