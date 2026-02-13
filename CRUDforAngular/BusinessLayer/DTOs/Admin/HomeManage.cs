using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.DTOs.Admin
{
    public class HomeManage
    { 
    }

    public class CarouselDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Image is missing")]
        public IFormFile Image { get; set; }
        [Required(ErrorMessage ="Title is missing")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is missing")]
        public string Description { get; set; } = string.Empty;
        
    }

    public class CategoryDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Image is missing")]
        public IFormFile Image { get; set; }
        [Required(ErrorMessage = "Title is missing")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is missing")]
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

    }

    public class  CategoryItem
    {
        public int Id { get; set; }
     
        [Required(ErrorMessage = "Title is missing")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is missing")]
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

    }


}
