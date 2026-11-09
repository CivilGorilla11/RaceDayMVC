using Microsoft.AspNetCore.Mvc;
using RaceDay.Filters;
using RaceDay.Helpers;
using RaceDay.Models.Results;
using RaceDay.Services.Interfaces;

namespace RaceDay.Controllers;

[RoleRequired(Roles.Participant)]
public class ResultsController : Controller
{
    private readonly IResultApiService _resultService;

    public ResultsController(IResultApiService resultService)
    {
        _resultService = resultService;
    }

    // GET /Results/Mine -> my personal race history
    public async Task<IActionResult> Mine()
    {
        var result = await _resultService.GetMineAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return View(new List<ResultViewModel>());
        }

        // Newest first. Results with no date from the API sort to the bottom.
        return View(result.Data!.OrderByDescending(r => r.EventDate ?? DateTime.MinValue).ToList());
    }
}