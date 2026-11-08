using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models.Categories
{
    public class CategoryFormViewModel
    {
        public int CategoryId { get; set; } // 0 when adding
        public int EventId { get; set; }    // which event this category belongs to (kept in a hidden field)

        [Required(ErrorMessage = "Please enter a category name.")]
        [StringLength(100, ErrorMessage = "Name can be at most 100 characters.")]
        [Display(Name = "Category name")]
        public string CategoryName { get; set; } = string.Empty;

        // [Range] blocks nonsense values. Note: the SQL column is DECIMAL(5,2), so the max distance is 999.99.
        [Range(0.01, 999.99, ErrorMessage = "Distance must be between 0.01 and 999.99 km.")]
        [Display(Name = "Distance (km)")]
        public decimal Distance { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "Entry fee cannot be negative.")]
        [Display(Name = "Entry fee (R)")]
        public decimal EntryFee { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "There must be room for at least 1 participant.")]
        [Display(Name = "Maximum participants")]
        public int MaxParticipants { get; set; }

    }
}
