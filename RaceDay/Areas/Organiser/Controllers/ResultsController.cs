using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Models.Enrolments;
using RaceDay.Models.Results;
using RaceDay.Services.Interfaces;
using RaceDay.Models.Events;
using RaceDay.Filters;
namespace RaceDay.Areas.Organiser.Controllers
{
    [Area("Organiser")]
    [RoleRequired("Organiser")]

    public class ResultsController : Controller
    {
        private readonly IResultApiService _resultService;
        private readonly IEnrolmentApiService _enrolmentService;
        private readonly IEventApiService _eventService;

        public ResultsController(
            IResultApiService resultService,
            IEnrolmentApiService enrolmentService,
            IEventApiService eventService)
        {
            _resultService = resultService;
            _enrolmentService = enrolmentService;
            _eventService = eventService;
        }


        // GET: ResultsController
        public async Task<ActionResult>Index(int eventId)
        {
            ViewBag.EventId = eventId;
            ViewBag.EventName = await GetEventNameAsync(eventId);

            var enrolmentsResult = await _enrolmentService.GetForEventAsync(eventId);
            var resultsResult = await _resultService.GetForEventAsync(eventId);

            if (!enrolmentsResult.Success)
            {
                TempData["ErrorMessage"] = enrolmentsResult.ErrorMessage;
                return View(new List<EnrolmentViewModel>());
            }
            ViewBag.ResultsByEnrolment = resultsResult.Success
                ? resultsResult.Data!.GroupBy(r => r.EnrolmentId).ToDictionary(g => g.Key, g => g.First())
                : new Dictionary<int, ResultViewModel>();

            return View(enrolmentsResult.Data);
        }

        
        // GET: ResultsController/Details/5
        public async Task<ActionResult> Create(int eventId, int enrolmentId)
        {
            var form = new ResultFormViewModel
            {
                EventId = eventId,
                EnrolmentId = enrolmentId,
                Race = await GetEventNameAsync(eventId)   // sensible default; the organiser can change it
            };
            return View(form);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ResultFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _resultService.CreateAsync(model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }
            TempData["SuccessMessage"] = "Result created successfully.";
            return RedirectToAction(nameof(Index), new { eventId = model.EventId });
        }

        // GET: ResultsController/Create
        public async Task<ActionResult> Edit(int eventId, int Id)
        {
            var result = await _resultService.GetForEventAsync(eventId);
            var existing = result.Success ? result.Data!.FirstOrDefault(r => r.Id == Id) : null;

            if (existing == null)
            {
                TempData["ErrorMessage"] = "Result not found.";
                return RedirectToAction(nameof(Index), new { eventId });
            }
            var form = new ResultFormViewModel
            {
                ResultId = existing.Id,
                EventId = existing.EventId,
                EnrolmentId = existing.EnrolmentId,
                Race = existing.Race,
                FinishTime = existing.FinishTime,
                Position = existing.Position,
            };
            return View(form);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ResultFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _resultService.UpdateAsync(model.ResultId, model);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Result updated.";
            return RedirectToAction(nameof(Index), new { eventId = model.EventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            var result = await _resultService.DeleteAsync(id);

            if (result.Success)
                TempData["SuccessMessage"] = "Result removed.";
            else
                TempData["ErrorMessage"] = result.ErrorMessage;

            return RedirectToAction(nameof(Index), new { eventId });
        }

        private async Task<string> GetEventNameAsync(int eventId)
        {
            var result = await _eventService.GetEventAsync(eventId);
            return result.Success ? result.Data!.EventName : "Event";
        }
    }
}






