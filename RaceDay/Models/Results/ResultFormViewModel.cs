using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models.Results
{
    public class ResultFormViewModel
    {
        public int ResultId { get; set; } // 0 when creating; used when editing

        public int EventId { get; set; }  // which event this result belongs to (hidden field)
        public int EnrolmentId { get; set; }  // which enrolment this result belongs to (hidden field)

        [Required(ErrorMessage = "Please enter the finish time.")]
        [Display(Name = "Finish time (hh:mm:ss)")]
        public TimeSpan FinishTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Position must be 1 or higher.")]
        public int Position { get; set; }

        [Required(ErrorMessage = "Please enter the race name.")]
        [StringLength(100, ErrorMessage = "Race can be at most 100 characters.")]
        public string Race { get; set; } = string.Empty;
    }
}
