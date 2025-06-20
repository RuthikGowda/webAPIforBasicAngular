using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public class userRegistrationRepo : IuserRegistrationRepo
    {
        private readonly MyDBContext _context;
        public userRegistrationRepo(MyDBContext context)
        {
            _context = context;
        }

        public int RegisterUser(userRegistration userCred)
        {
            try
            {
                if (_context.UserInfo.Any(u => u.Email == userCred.Email))
                {
                    return -1; // Email already exists
                }
                var hasher = new PasswordHasher<userRegistration>();
                userCred.Password = hasher.HashPassword(userCred, userCred.Password);
                UserInfo user = new UserInfo
                {
                    UserRegistration = userCred
                };
                user.Email = userCred.Email;

                _context.user.Add(user);
                _context.SaveChanges();
                return userCred.Id; // Return the ID of the newly created user


            }
            catch (Exception  )
            {
                // Log the exception (ex) as needed
                return -2; // Indicate an error occurred
            }
        }

        public int LoginUser(LoginModel loginCred)
        {
            try
            {
                var Exist = _context.user.Any(u => u.Email == loginCred.Email);
                if (!Exist)
                {
                    return -1;// user not found
                }

                var dbPassword = _context.user.Where(u => u.Email == loginCred.Email)
                    .Select(u => u.UserRegistration.Password)
                    .FirstOrDefault();
                var hasher = new PasswordHasher<LoginModel>();
                var password = hasher.VerifyHashedPassword(loginCred, dbPassword ?? "", loginCred.Password);

                return password.ToString() == "Success" ? 1 : 0; // Login successful, return 1 or any other success code
            }
            catch (Exception  )
            {
                // Log the exception (ex) as needed
                return -2; // Indicate an error occurred
            }
        }


        public async Task<bool> ValidateEmail(string email)
        {
            try
            {
                // Check if the email exists in the database
                return await _context.user.AsNoTracking().AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
