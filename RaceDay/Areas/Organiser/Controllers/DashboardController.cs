using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Dashboard;
using RaceDay.Models;
using RaceDay.Services.Interfaces;
using RaceDay.Filters;

namespace RaceDay.Web.Areas.Organiser.Controllers; 

[Area("Organiser")]            // tells MVC this controller lives in the Organiser area (URL: /Organiser/Dashboard)
[RoleRequired("Organiser")]    // your filter: blocks anyone who is not logged in as an Organiser
public class DashboardController : Controller
{
    // The controller asks for the INTERFACES ("the menu"). Program.cs decides which class fills them.
    private readonly IEventApiService _eventService;
    private readonly IEnrolmentApiService _enrolmentService;

    public DashboardController(IEventApiService eventService, IEnrolmentApiService enrolmentService)
    {
        _eventService = eventService;
        _enrolmentService = enrolmentService;
    }

    // GET /Organiser/Dashboard
    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel(); // starts with an empty list, so the view never hits null

        var eventsResult = await _eventService.GetMyEventsAsync();
        if (!eventsResult.Success)
        {
            // Show a friendly message (the layout prints TempData) and an empty dashboard. No crash.
            TempData["ErrorMessage"] = eventsResult.ErrorMessage;
            return View(model);
        }

        // "Data!" means: "I know it is not null, because Success was true".
        foreach (var ev in eventsResult.Data!.OrderBy(e => e.EventDate))
        {
            var enrolments = await _enrolmentService.GetForEventAsync(ev.EventId);

            model.Events.Add(new DashboardEventItem
            {
                EventId = ev.EventId,
                EventName = ev.EventName,
                EventDate = ev.EventDate,
                TotalEnrolments = enrolments.Success ? enrolments.Data!.Count : 0
            });
        }

        // OPTIMIZE: this makes one API call per event (the "N+1 problem"). Fine for 3 events.
        // Later you can start all the calls at once with Task.WhenAll, or add one summary endpoint to the API.
        return View(model);
    }
}
