using CRUDforAngular.BusinessLayer.Models;

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
}
