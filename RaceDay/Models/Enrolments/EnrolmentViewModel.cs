namespace RaceDay.Models.Enrolments
{
    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public int CategoryId { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty; // "Confirmed" or "Pending" (the colour badge reads this)

    }
}
