using System.ComponentModel.DataAnnotations;

namespace RaceDay.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "That does not look like a valid email address.")]
        public string Email { get; set; } = string.Empty; // "= string.Empty" avoids null warnings

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)] // renders the input as a password box (dots)
        public string Password { get; set; } = string.Empty;

    }
}
