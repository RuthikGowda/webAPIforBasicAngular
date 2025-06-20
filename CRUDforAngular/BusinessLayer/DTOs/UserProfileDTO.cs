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

        public DateOnly DateOfBirth { get; set; }

        public IList<Phone> Phone { get; set; } = new List<Phone>();

       
        public IList<Address> Address { get; set; } = new List<Address>();
     
    }
}
