using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Enrolments;
using RaceDay.Models;
using RaceDay.Services.Interfaces;
using RaceDay.Filters;
namespace RaceDay.Web.Areas.Organiser.Controllers;

[Area("Organiser")]
[RoleRequired("Organiser")]
public class EnrolmentsController : Controller
{
    private readonly IEnrolmentApiService _enrolmentService;
    private readonly IEventApiService _eventService;

    public EnrolmentsController(IEnrolmentApiService enrolmentService, IEventApiService eventService) 
    {
        _enrolmentService = enrolmentService;
        _eventService = eventService;
    }

    // GET /Organiser/Enrolments?eventId=1  -> everyone enrolled in this event, with their status
    public async Task<IActionResult> Index(int eventId)
    {
        ViewBag.EventId = eventId;

        var eventResult = await _eventService.GetEventAsync(eventId);
        ViewBag.EventName = eventResult.Success ? eventResult.Data!.EventName : "Event";

        var result = await _enrolmentService.GetForEventAsync(eventId);
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<EnrolmentViewModel>());
        }

        // The view shows each Status through the _StatusBadge partial (green Confirmed, amber Pending).
        return View(result.Data);
    }


    // NOTE: the API returns ids, not names. If the view only shows "Participant 2 / Category 5",
    // add ParticipantName and CategoryName to the API's response (see the comments in EnrolmentViewModel).
}