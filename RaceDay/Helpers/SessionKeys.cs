namespace RaceDay.Helpers;

// Constants so you never mistype a session key like "Jwt" vs "JWT".
public static class SessionKeys
{
    public const string Jwt = "JWT";
    public const string Role = "Role";
    public const string UserEmail = "UserEmail";
} 

// The exact role strings your API puts in its login response.
public static class Roles
{
    public const string Organiser = "Organiser";
    public const string Participant = "Participant";
}
