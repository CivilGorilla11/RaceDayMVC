using Microsoft.AspNetCore.Http;

namespace RaceDay.Helpers;

// EXTENSION METHODS: they add new methods to ISession, so you can write
// HttpContext.Session.IsOrganiser() anywhere. (Named "...UserExtensions" because the
// framework already has a class called SessionExtensions, and two with the same name confuse the compiler.)
public static class SessionUserExtensions
{
    // Called once, right after a successful login.
    public static void SaveLogin(this ISession session, string token, string role, string email)
    {
        session.SetString(SessionKeys.Jwt, token);
        session.SetString(SessionKeys.Role, role);
        session.SetString(SessionKeys.UserEmail, email);   // for "Logged in as ..." in the nav. NEVER store the password.
    }

    public static string? GetToken(this ISession session) => session.GetString(SessionKeys.Jwt);
    public static string? GetRole(this ISession session) => session.GetString(SessionKeys.Role);
    public static string? GetUserEmail(this ISession session) => session.GetString(SessionKeys.UserEmail);

    // Logged in = we are holding a token.
    public static bool IsLoggedIn(this ISession session) => !string.IsNullOrEmpty(session.GetToken());

    // OrdinalIgnoreCase = "organiser" and "Organiser" both match, so a case difference cannot lock you out.
    public static bool IsOrganiser(this ISession session) =>
        string.Equals(session.GetRole(), Roles.Organiser, StringComparison.OrdinalIgnoreCase);

    public static bool IsParticipant(this ISession session) =>
        string.Equals(session.GetRole(), Roles.Participant, StringComparison.OrdinalIgnoreCase);
}
