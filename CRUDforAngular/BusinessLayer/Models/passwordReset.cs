using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDforAngular.BusinessLayer.Models
{
    [PrimaryKey(nameof(id))]
    public class passwordReset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email")]
        public string email { get; set; } = string.Empty;
        [Required]
        public string resetId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime createdDttm { get; set; } = DateTime.UtcNow; // Default expiry time is 1 hour from now.

        public bool isUsed { get; set; } = false; // Default value is false, indicating the reset link has not been used yet.
        public bool isexpired { get; set; } = false;


    }
}
