using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.BusinessLayer.Models
{
    public class Attendee
    {
        
            public int Id { get; set; }

            public string FullName { get; set; }

            [EmailAddress]
            public string Email { get; set; }

            public int EventId { get; set; }

            public Events Event { get; set; }

            public int UserId { get; set; }








    }
}
