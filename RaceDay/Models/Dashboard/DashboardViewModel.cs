namespace RaceDay.Models.Dashboard
{
    public class DashboardViewModel
    {
        public List<DashboardEventItem> Events { get; set; } = new(); // start empty so the view never hits null
        public int TotalEvents => Events.Count;                       // calculated, nothing to set
        public int TotalEnrolments => Events.Sum(e => e.TotalEnrolments);
    }

    // One row on the dashboard. It lives in the same file because nothing else uses it.
    public class DashboardEventItem
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public int TotalEnrolments { get; set; }

    }
}
