using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RaceDay.Helpers;

namespace RaceDay.Filters;

// ACTION FILTER: runs BEFORE a controller action.
//   [RoleRequired("Organiser")]  -> must be logged in AND be an Organiser
//   [RoleRequired]               -> must be logged in, any role
// This is for user experience only. The REAL security is the API's [Authorize(Roles=...)].
public class RoleRequiredAttribute : ActionFilterAttribute
{
    private readonly string? _role;

    public RoleRequiredAttribute(string? role = null)
    {
        _role = role;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;

        // Not logged in -> Login page. (area = "" means "the main site, not the Organiser area")
        if (!session.IsLoggedIn())
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;   // setting context.Result stops the action from running
        }

        // Logged in but the wrong role -> Access Denied page.
        if (_role != null && !string.Equals(session.GetRole(), _role, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "" });
        }
    }

    // OPTIMIZE: remember the page the user wanted and send them back to it after login (a "returnUrl").
}