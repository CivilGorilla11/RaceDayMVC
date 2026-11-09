namespace RaceDay.Helpers;

// Turns 47 into "47th", 1 into "1st", 22 into "22nd". Used on the My Results page.
public static class NumberExtensions
{
    public static string ToOrdinal(this int number)
    {
        var lastTwo = number % 100;
        if (lastTwo >= 11 && lastTwo <= 13)       // 11th, 12th, 13th are the exceptions
            return number + "th";

        return (number % 10) switch
        {
            1 => number + "st",
            2 => number + "nd",
            3 => number + "rd",
            _ => number + "th"
        };
    }
}
