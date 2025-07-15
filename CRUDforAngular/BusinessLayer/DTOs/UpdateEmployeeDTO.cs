using CRUDforAngular.BusinessLayer.Models;
using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.DTOs
{
    public class UpdateEmployeeDTO
    {

        public string id { get; set; } = string.Empty;
        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public DateOnly dateOfBirth { get; set; }
        public IList<Phone> phone { get; set; } = new List<Phone>();
        public IList<Address> address { get; set; } = new List<Address>();
    }

    public class validateResetPasswordDto
    {
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string email { get; set; } = string.Empty;
        [Required]
        public string resetId { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required]
        public string resetId { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password, ErrorMessage = "Invalid Password")]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string confirmPassword { get; set; } = string.Empty; 
    }
}
