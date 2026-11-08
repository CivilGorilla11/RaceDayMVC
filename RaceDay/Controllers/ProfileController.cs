using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Filters;
using RaceDay.Helpers;
using RaceDay.Models.Profile;
using RaceDay.Services.Interfaces;

namespace RaceDay.Controllers;

[RoleRequired(Roles.Participant)]
public class ProfileController : Controller
{
    private readonly IProfileApiService _profileService;

    public ProfileController(IProfileApiService profileService)
    {
        _profileService = profileService;
    }

    // GET /Profile -> show my details
    public async Task<IActionResult> Index()
    {
        var result = await _profileService.GetMeAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction("Index", "Home");
        }

        return View(result.Data);
    }

    // GET /Profile/Edit -> the form, pre-filled
    public async Task<IActionResult> Edit()
    {
        var result = await _profileService.GetMeAsync();
        if (!result.Success)
        {
            TempData["ErrorMessage"] = result.ErrorMessage;
            return RedirectToAction(nameof(Index));
        }

        var form = result.Data!;
        form.Password = null;   // never pre-fill a password box
        return View(form);
    }

    // POST /Profile/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _profileService.UpdateMeAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        // Keep the email shown in the nav menu in sync with the change.
        HttpContext.Session.SetString(SessionKeys.UserEmail, model.Email);

        TempData["SuccessMessage"] = "Your profile has been updated.";
        return RedirectToAction(nameof(Index));
    }

    // LATER: profile picture upload (needs a new API endpoint + Azure Blob Storage).
}