using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class Events
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Location { get; set; }

        public int OrganizerId { get; set; }

        
    }

}
