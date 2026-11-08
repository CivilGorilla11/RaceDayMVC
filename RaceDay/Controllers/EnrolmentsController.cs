using Microsoft.AspNetCore.Mvc;
using RaceDay.Filters;
using RaceDay.Helpers;
using RaceDay.Models.Enrolments;
using RaceDay.Services.Interfaces;

namespace RaceDay.Controllers;

[RoleRequired(Roles.Participant)]   // organisers are sent to Access Denied
public class EnrolmentsController : Controller
{
    private readonly IEnrolmentApiService _enrolmentService;

    public EnrolmentsController(IEnrolmentApiService enrolmentService)
    {
        _enrolmentService = enrolmentService;
    }

    // GET /Enrolments/Mine -> my enrolments and their status
    public async Task<IActionResult> Mine()
    {
        var result = await _enrolmentService.GetMineAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<EnrolmentViewModel>());
        }

        return View(result.Data!.OrderByDescending(e => e.EnrolmentDate).ToList());
    }

    // POST /Enrolments/Create -> the "Enrol" button on the event details page
    // categoryId = the chosen category, eventId = so we can go back to the event if it fails
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int categoryId, int eventId)
    {
        var result = await _enrolmentService.EnrolAsync(categoryId);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        TempData["SuccessMessage"] = "You are enrolled. Your enrolment is shown below.";
        return RedirectToAction(nameof(Mine));
    }

    // POST /Enrolments/Delete/5 -> withdraw
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _enrolmentService.WithdrawAsync(id);

        if (result.Success)
            TempData["SuccessMessage"] = "You have withdrawn from that event.";
        else
            TempData["ErrorMessage"] = result.ErrorMessage;

        return RedirectToAction(nameof(Mine));
    }
}