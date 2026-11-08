using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Events;
using RaceDay.Services.Interfaces;

namespace RaceDay.Controllers;

// Public on purpose: guests can read event details, but only a logged-in participant can enrol.
public class EventsController : Controller
{
    private readonly IEventApiService _eventService;
    private readonly ICategoryApiService _categoryService;

    public EventsController(IEventApiService eventService, ICategoryApiService categoryService)
    {
        _eventService = eventService;
        _categoryService = categoryService;
    }

    // GET /Events/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var eventResult = await _eventService.GetEventAsync(id);
        if (!eventResult.Success)
        {
            TempData["ErrorMessage"] = eventResult.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        var model = new EventDetailsViewModel { Event = eventResult.Data! };

        // If categories fail to load we still show the event, with an error message.
        var categoriesResult = await _categoryService.GetForEventAsync(id);
        if (categoriesResult.Success)
            model.Categories = categoriesResult.Data!;
        else
            TempData["ErrorMessage"] = categoriesResult.ErrorMessage;

        return View(model);
    }
}