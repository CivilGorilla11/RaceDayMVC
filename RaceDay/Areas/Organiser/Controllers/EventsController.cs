using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Events;
using RaceDay.Models;
using RaceDay.Services.Interfaces;
using RaceDay.Filters;
namespace RaceDay.Web.Areas.Organiser.Controllers;

[Area("Organiser")]
[RoleRequired("Organiser")]
public class EventsController : Controller
{
    private readonly IEventApiService _eventService;

    public EventsController(IEventApiService eventService)
    {
        _eventService = eventService;
    }

    // GET /Organiser/Events  -> list of MY events
    public async Task<IActionResult> Index()
    {
        var result = await _eventService.GetMyEventsAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<EventViewModel>());
        }
        return View(result.Data);   // the view's @model is List<EventViewModel>
    }

    // GET /Organiser/Events/Create  -> show an empty form
    public IActionResult Create()
    {
        return View(new EventFormViewModel());
    }

    // POST /Organiser/Events/Create  -> the form was submitted
    [HttpPost]
    [ValidateAntiForgeryToken] // blocks forged form posts; <form asp-action=...> adds the token automatically
    public async Task<IActionResult> Create(EventFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);   // show the form again with the validation messages

        var result = await _eventService.CreateEventAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage); // shown in the validation summary
            return View(model);                                          // keeps what the user typed
        }

        TempData["SuccessMessage"] = "Event created successfully.";
        return RedirectToAction(nameof(Index)); // Post-Redirect-Get: refreshing will not resubmit the form
    }

    // GET /Organiser/Events/Edit/5  -> form pre-filled with the existing event
    public async Task<IActionResult> Edit(int id)
    {
        var result = await _eventService.GetEventAsync(id);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        // Copy the display model into the form model (they are different classes on purpose).
        var e = result.Data!;
        var form = new EventFormViewModel
        {
            EventId = e.EventId,
            EventName = e.EventName,
            EventDate = e.EventDate,
            Address = e.Address,
            Description = e.Description
        };
        return View(form);
    }

    // POST /Organiser/Events/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EventFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _eventService.UpdateEventAsync(model.EventId, model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["SuccessMessage"] = "Event updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Organiser/Events/Delete/5
    // Delete is POST-only (never GET), so a link or a search engine can never delete something.
    // In the view, use a small form with a button and onsubmit="return confirm('Delete this event?')".
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _eventService.DeleteEventAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "Event deleted.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index));
    }

    // OPTIMIZE: the attributes at the top of every controller repeat. Later, create
    // "OrganiserBaseController" with [Area] and [RoleRequired] once, and inherit from it.
}
