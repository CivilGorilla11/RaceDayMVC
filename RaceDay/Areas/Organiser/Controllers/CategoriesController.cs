using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Categories;

using RaceDay.Models;
using RaceDay.Services.Interfaces;
using RaceDay.Filters;

namespace RaceDay.Web.Areas.Organiser.Controllers;

[Area("Organiser")]
[RoleRequired("Organiser")]
public class CategoriesController : Controller
{
    private readonly ICategoryApiService _categoryService;
    private readonly IEventApiService _eventService;

    public CategoriesController(ICategoryApiService categoryService, IEventApiService eventService)
    { 
        _categoryService = categoryService;
        _eventService = eventService;
    }

    // GET /Organiser/Categories?eventId=1  -> categories of ONE event
    public async Task<IActionResult> Index(int eventId)
    {
        // ViewBag carries small extra values to the view next to the main model.
        ViewBag.EventId = eventId;
        ViewBag.EventName = await GetEventNameAsync(eventId);

        var result = await _categoryService.GetForEventAsync(eventId);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<CategoryViewModel>());
        }
        return View(result.Data);
    }

    // GET /Organiser/Categories/Create?eventId=1
    public IActionResult Create(int eventId)
    {
        // Put the event id in the form model so it travels with the POST (hidden field in the view).
        return View(new CategoryFormViewModel { EventId = eventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _categoryService.CreateAsync(model.EventId, model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["SuccessMessage"] = "Category added.";
        return RedirectToAction(nameof(Index), new { eventId = model.EventId });
    }

    // GET /Organiser/Categories/Edit?eventId=1&id=3
    public async Task<IActionResult> Edit(int eventId, int id)
    {
        // Your Part 1 plan has no "get ONE category" endpoint, so fetch the event's list and pick the right one.
        var result = await _categoryService.GetForEventAsync(eventId);
        var category = result.Success ? result.Data!.FirstOrDefault(c => c.CategoryId == id) : null;

        if (category == null)
        {
            TempData["ErrorMessage"] = "That category could not be found.";
            return RedirectToAction(nameof(Index), new { eventId });
        }

        var form = new CategoryFormViewModel
        {
            CategoryId = category.CategoryId,
            EventId = category.EventId,
            CategoryName = category.CategoryName,
            Distance = category.Distance,
            EntryFee = category.EntryFee,
            MaxParticipants = category.MaxParticipants
        };
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _categoryService.UpdateAsync(model.CategoryId, model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        TempData["SuccessMessage"] = "Category updated.";
        return RedirectToAction(nameof(Index), new { eventId = model.EventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int eventId)
    {
        var result = await _categoryService.DeleteAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "Category removed.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Index), new { eventId });
    }

    // Small private helper: the page heading should show the event name, not just a number.
    private async Task<string> GetEventNameAsync(int eventId)
    {
        var result = await _eventService.GetEventAsync(eventId);
        return result.Success ? result.Data!.EventName : "Event";
    }
}

