using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Required password length between 8 to 20.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}
