using Microsoft.AspNetCore.Mvc;
using RaceDay.Helpers;
using RaceDay.Models.Account;
using RaceDay.Services.Interfaces;


namespace RaceDay.Controllers;

// No [RoleRequired] here: guests must be able to reach login and register.
public class AccountController : Controller
{
    private readonly IAuthApiService _authService;

    public AccountController(IAuthApiService authService)
    {
        _authService = authService;
    }

    // ---------- LOGIN (one form for both roles; the API says which role you are) ----------

    // GET /Account/Login -> show the form
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    // POST /Account/Login -> the form was submitted
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authService.LoginAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View(model);
        }

        // Keep the token and role in session. JwtHandler attaches the token to later API calls.
        var login = result.Data!;
        HttpContext.Session.SaveLogin(login.Token, login.Role, model.Email);

        // Each role lands on its own page.
        if (HttpContext.Session.IsOrganiser())
            return RedirectToAction("Index", "Dashboard", new { area = "Organiser" });

        return RedirectToAction("Index", "Home", new { area = "" });
    }

    // ---------- PARTICIPANT REGISTRATION ----------

    // GET /Account/Register
    public IActionResult Register()
    {
        ViewData["AccountType"] = "Participant";      // the shared Register view reads these two values
        ViewData["FormAction"] = nameof(Register);
        return View("Register", new RegisterViewModel());
    }

    // POST /Account/Register -> calls the participant register endpoint
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        ViewData["AccountType"] = "Participant";
        ViewData["FormAction"] = nameof(Register);

        if (!ModelState.IsValid)
            return View("Register", model);

        var result = await _authService.RegisterParticipantAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View("Register", model);
        }

        TempData["SuccessMessage"] = "Account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    // ---------- ORGANISER REGISTRATION (same view, different API endpoint) ----------

    // GET /Account/RegisterOrganiser
    public IActionResult RegisterOrganiser()
    {
        ViewData["AccountType"] = "Organiser";
        ViewData["FormAction"] = nameof(RegisterOrganiser);
        return View("Register", new RegisterViewModel());
    }

    // POST /Account/RegisterOrganiser -> calls the organiser register endpoint
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterOrganiser(RegisterViewModel model)
    {
        ViewData["AccountType"] = "Organiser";
        ViewData["FormAction"] = nameof(RegisterOrganiser);

        if (!ModelState.IsValid)
            return View("Register", model);

        var result = await _authService.RegisterOrganiserAsync(model);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return View("Register", model);
        }

        TempData["SuccessMessage"] = "Organiser account created. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    // ---------- LOGOUT / ACCESS DENIED ----------

    // POST only. The nav menu uses a small form with a Logout button.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();   // wipes token, role and email
        return RedirectToAction(nameof(Login));
    }

    // GET /Account/AccessDenied -> shown by RoleRequiredAttribute for the wrong role
    public IActionResult AccessDenied()
    {
        return View();
    }
}