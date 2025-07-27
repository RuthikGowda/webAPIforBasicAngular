using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class userRegistration
    {

        [BindNever]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters.")]
         
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [StringLength(15, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; } = string.Empty;

        [BindNever]
        public string? OTP { get; set; } = string.Empty;
        [DataType(DataType.DateTime)]
        public DateTime? createdDateTime { get; set; } = DateTime.UtcNow;
        public bool isVerified { get; set; } = false;
 

    }
}
