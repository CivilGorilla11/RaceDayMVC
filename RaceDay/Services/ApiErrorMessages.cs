using System.Net;

namespace RaceDay.Services;

// SHARED HELPER: friendly messages used by all four service classes.
// "static" means you never create one with "new"; you call ApiErrorMessages.For(...) directly.
public static class ApiErrorMessages
{
    // Used when the API cannot be reached at all (API stopped, wrong port).
    public const string Unreachable = "Could not connect to the RaceDay API. Please try again later.";

    // Turns an HTTP status code from the API into text a normal person can read.
    public static string For(HttpStatusCode code) => code switch
    {
        HttpStatusCode.BadRequest => "The information submitted was not valid.",
        HttpStatusCode.Unauthorized => "Your session has expired. Please log in again.",
        HttpStatusCode.Forbidden => "You do not have permission to do that.",
        HttpStatusCode.NotFound => "That item could not be found.",
        _ => "Something went wrong. Please try again."
    };
}
