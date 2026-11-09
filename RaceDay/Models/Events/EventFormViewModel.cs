using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace RaceDay.Models.Events

{
    public class EventFormViewModel
    {
        public int EventId { get; set; } // 0 when creating; filled in when editing

        [Required(ErrorMessage = "Please enter the event name.")]
        [StringLength(100, ErrorMessage = "Event name can be at most 100 characters.")] // matches SQL VARCHAR(100)
        [Display(Name = "Event name")] // the label text shown on the form
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please choose the event date.")]
        [DataType(DataType.Date)] // gives you a date picker
                                  // OPTIMIZE: without this format the date picker may show blank when editing an existing event
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Event date")]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please enter the location.")]
        [StringLength(255, ErrorMessage = "Address can be at most 255 characters.")] // matches SQL VARCHAR(255)
        [JsonPropertyName("eventAddress")] // matches the API's JSON property name
        public string Address { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description can be at most 500 characters.")] // matches SQL VARCHAR(500)
        public string? Description { get; set; } // optional, so no [Required]

         // ADD LATER for the banner upload (Part 3, Azure Blob):
         public IFormFile? BannerImage { get; set; }

        public string? BannerUrl { get; set; } // optional, so no [Required]

    }
}
