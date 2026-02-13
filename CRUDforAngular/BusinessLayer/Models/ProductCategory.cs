using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class ProductCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int categoryId { get; set; }

        public string categoryName { get; set; } = string.Empty;
        public string categoryDescription { get; set; } = string.Empty;

        public string Imageurl { get; set; } = string.Empty;


    }
}
