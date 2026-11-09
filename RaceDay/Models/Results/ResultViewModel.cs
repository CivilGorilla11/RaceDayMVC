using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models.Results
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }

        public int EventId { get; set; }
        [Required(ErrorMessage = "Please enter the finish time.")]
        [Display(Name = "Finish time (hh:mm:ss)")]
        public TimeSpan FinishTime { get; set; } // SQL TIME maps to TimeSpan (e.g. 01:45:30)

        [Range(1, int.MaxValue, ErrorMessage = "Position must be 1 or higher.")]
        public int Position { get; set; }

        [Required(ErrorMessage = "Please enter the race name.")]
        [StringLength(100, ErrorMessage = "Race can be at most 100 characters.")]
        public string Race { get; set; } = string.Empty;

        public int Id { get; internal set; } 

        public string? EventName { get; set; }
        public DateTime? EventDate { get; set; }
        public string? CategoryName { get; set; }
        public int? TotalFinishers { get; set; }
    }
}
