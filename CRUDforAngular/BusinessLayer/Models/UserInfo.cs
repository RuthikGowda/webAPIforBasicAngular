using Microsoft.EntityFrameworkCore.Query.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
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
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }

      

        public IList<Phone> Phone { get; set; } = new List<Phone>();

        //create navigation property for user address
         

        public IList<Address> Address { get; set; } = new List<Address>(); 

        //create navigation property for user registration  
        [ForeignKey(nameof(UserRegistration))]
        public int UserRegistrationId { get; set; }
        public required userRegistration UserRegistration { get; set; }

        public ICollection<Events> Events { get; set; }

    }

    public class Phone
    {
        public int Id { get; set; }
        public long PhoneNumber { get; set; }

        public string PhoneNumberType { get; set; } = string.Empty;

        [ForeignKey(nameof(UserInfo))]
        public int UserInfoId { get; set; }



    }

    public class Address
    {
        public int Id { get; set; }
        public string AddressType { get; set; }
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; }

        [ForeignKey(nameof(UserInfo))]
        public int UserInfoId { get; set; }
    }
}
