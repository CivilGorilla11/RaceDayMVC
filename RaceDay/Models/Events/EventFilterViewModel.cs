namespace RaceDay.Models.Events
{
    // FILTER MODEL: the search boxes on the home page. Matches GET /api/events?search=&location=&date=
    // Everything is nullable because every filter is optional.

    public class EventFilterViewModel
    {
        public string? Search { get; set; }
        public string? Location { get; set; }
        public DateTime? Date { get; set; } // "DateTime?" = a date or nothing

    }
}
