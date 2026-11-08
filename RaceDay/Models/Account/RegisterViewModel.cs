using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter your name.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2 to 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "That does not look like a valid email address.")]
        public string Email { get; set; } = string.Empty;

        // Part 1 SQL has Password VARCHAR(20), hence the 20 max. Match whatever rule your API enforces.
        [Required(ErrorMessage = "Please choose a password.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be 6 to 20 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")] // must equal Password
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
