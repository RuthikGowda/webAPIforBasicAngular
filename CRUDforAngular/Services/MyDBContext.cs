using CRUDforAngular.BusinessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUDforAngular.Services
{
    public class MyDBContext : DbContext
    {    
        public DbSet<userRegistration> UserInfo { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }
        public DbSet<UserInfo> user { get; set; }

        public DbSet<Phone> UserPhones { get; set; }
        public DbSet<Address> UserAddress { get; set; }   

        public DbSet<passwordReset> passwordResets { get; set; }
        public MyDBContext(DbContextOptions<MyDBContext> options)
                : base(options)
        {

        }

    }


}
