using System.ComponentModel.DataAnnotations;
namespace RaceDay.Models.Profile
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Name cannot be empty.")]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email cannot be empty.")]
        [EmailAddress(ErrorMessage = "That does not look like a valid email address.")]
        public string Email { get; set; } = string.Empty;

        // Left blank = "do not change my password". NEVER prefill this field from the API.
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6 to 20 characters.")]
        [Display(Name = "New password (optional)")]
        public string? Password { get; set; }

        // ADD LATER for the profile picture (Part 3, Azure Blob):
        // public string? ProfilePictureUrl { get; set; }
        // public IFormFile? ProfilePicture { get; set; }
    }
}
