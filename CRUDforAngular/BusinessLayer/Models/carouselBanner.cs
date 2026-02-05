using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class carouselBanner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
          
        [Required( ErrorMessage ="ImageUrl is required")]
        public string ImageUrl { get; set; } = string.Empty;
      

        [Required(ErrorMessage ="Title is required")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
