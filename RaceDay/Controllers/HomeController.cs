using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Events;
using RaceDay.Services.Interfaces;

namespace RaceDay.Controllers;

public class HomeController : Controller
{
    private readonly IEventApiService _eventService;

    public HomeController(IEventApiService eventService)
    {
        _eventService = eventService;
    }

    // GET /?search=&location=&date=&showPast=true
    // [FromQuery] fills the EventFilterViewModel from the address bar, so filters are bookmarkable.
    public async Task<IActionResult> Index([FromQuery] EventFilterViewModel filter, bool showPast = false)
    {
        // The view needs these to keep the filter boxes filled in after searching.
        ViewBag.Filter = filter;
        ViewBag.ShowPast = showPast;

        var result = await _eventService.GetEventsAsync(filter);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<EventViewModel>());
        }

        IEnumerable<EventViewModel> events = result.Data!;

        // SAFETY NET: filter here too, in case your API ignores the query parameters.
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = filter.Search.Trim();
            events = events.Where(e =>
                e.EventName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                e.Address.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (e.Description ?? string.Empty).Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(filter.Location))
        {
            var place = filter.Location.Trim();
            events = events.Where(e => e.Address.Contains(place, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.Date.HasValue)
            events = events.Where(e => e.EventDate.Date >= filter.Date.Value.Date);

        // The brief says "upcoming events", so hide past ones unless the user ticks "show past".
        if (!showPast)
            events = events.Where(e => e.EventDate.Date >= DateTime.Today);

        return View(events.OrderBy(e => e.EventDate).ToList());
    }
}
