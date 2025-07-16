using CRUDforAngular.BusinessLayer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.DTOs
{
    public class UserProfileDTO
    {
        public int Id { get; set; }
          public string FirstName { get; set; } = string.Empty;
          public string LastName { get; set; } = string.Empty;
          public string Email { get; set; } = string.Empty;

        public DateOnly? DateOfBirth { get; set; }

        public IList<Phone> Phone { get; set; } = new List<Phone>();

       
        public IList<Address> Address { get; set; } = new List<Address>();
     
    }

    public class UserRegistrationDTO
    {
        [Required]
        [StringLength(15, ErrorMessage = "Last name cannot be longer than 15 characters.")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [StringLength(15, ErrorMessage = "Last name cannot be longer than 15 characters.")]
        public string LastName { get; set; } = string.Empty;
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

    }
}
