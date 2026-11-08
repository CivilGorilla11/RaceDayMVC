namespace RaceDay.Models.Account
{
    // RESPONSE MODEL: mirrors what POST /api/auth/login sends back: { "token", "role" }.
    // No validation attributes: we never type into this one, the API fills it.

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty; // the JWT; store it in session
        public string Role { get; set; } = string.Empty;  // exactly "Participant" or "Organiser"

    }
}
