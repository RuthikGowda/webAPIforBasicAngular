using CRUDforAngular.BusinessLayer.DTOs;
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
                if (_context.UserInfo.Any(u => u.Email.ToLower() == userCred.Email.ToLower()))
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
                //var Exist = _context.user.Any(u => u.Email == loginCred.Email);
                var Exist = _context.UserInfo.Any(u => u.Email.ToLower() == loginCred.Email.ToLower());
                if (!Exist)
                {
                    return -1;// user not found
                }

                var dbPassword = _context.UserInfo.Where(u => u.Email.ToLower() == loginCred.Email.ToLower())
                    .Select(u => u.Password)
                    .FirstOrDefault();

                  //var dbPassword = _context.user.Where(u => u.Email == loginCred.Email)
                  //  .Select(u => u.UserRegistration.Password)
                  //  .FirstOrDefault();
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
                return await _context.user.AsNoTracking().AnyAsync(u => u.Email.ToLower() == email.ToLower());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<(bool, string)> ValidateResetId(string email, string resetId)
        {
            try
            {
                var resetData = await _context.passwordResets.AsNoTracking()
                     .FirstOrDefaultAsync(u => u.email.ToLower() == email.ToLower() && u.resetId == resetId);

                if (resetData == null)
                    return (false, "<strong>Invalid action!</strong> Please try again.");
                if (resetData.isUsed || resetData.createdDttm > DateTime.Now.AddMinutes(10))
                    return (false, "This reset password link has been already used");
                if (resetData.isexpired)
                    return (false, "This reset password link has been already expired!");

               // resetData.isUsed = true; // Mark the reset ID as used

                //await _context.passwordResets
                //   .Where(p => p.email.ToLower() == email.ToLower() && p.resetId == resetId)
                //   .ExecuteUpdateAsync(u => u.SetProperty(p => p.isUsed, true));
                //await _context.SaveChangesAsync(); // Save changes to the database

                return (true, "Valid");
            }
            catch
            {
                // Log the exception as needed
                return (false, "An error occurred while validating the reset ID. Please try after sometime");
            }
  
        }

        public async Task<bool> savePasswordInfo(string email, string resetId)
        {
            try
            {
                passwordReset reset = new passwordReset
                {
                    email = email,
                    resetId = resetId.ToString(),
                   // createdDttm = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time")),
                    //isUsed = false,
                   // isexpired = false
                };
               // reset.createdDttm = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

                await _context.passwordResets.AddAsync(reset);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return false; // Indicate an error occurred
            }
           
        }

        public async Task<(bool, string)> resetPassword(ResetPasswordDto resetPasswordModel)
        {
            //check if this resetId is valid for email then check isexpired to true then update the isUsed
            try
            {
                var resetData = await _context.passwordResets.FirstOrDefaultAsync(u => u.email.ToLower() == resetPasswordModel.email.ToLower()
                && u.resetId == resetPasswordModel.resetId);
                if (resetData != null)
                {
                    if (resetData.isexpired || resetData.createdDttm <= DateTime.UtcNow.AddMinutes(-10))
                    {
                        return  (false, "This reset password link has been already expired!"); // Reset ID has expired
                    }
                    if(resetData.isUsed)
                    {
                        return (false, "This reset password link has been already used!"); // Reset ID has already been used
                    }
                    // Update the password
                    var user = await _context.UserInfo.FirstOrDefaultAsync(u => u.Email.ToLower() == resetPasswordModel.email.ToLower());
                    if (user != null)
                    {
                        var hasher = new PasswordHasher<ResetPasswordDto>();
                        user.confirmPassword = resetPasswordModel.confirmPassword; // Assuming you want to set confirmPassword as well
                        user.Password = hasher.HashPassword(resetPasswordModel, resetPasswordModel.Password);
                        _context.SaveChanges();
                        // Mark the reset ID as used and expired
                        resetData.isUsed = true;
                        resetData.isexpired = true;
                        _context.SaveChanges();
                        return (true, "Password reset successful");  // Password reset successful
                    }
                }
                return (false, "<strong>Invalid action!</strong> Please try again."); // Reset ID not found or user not found
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return (false, "An error occurred while validating the reset ID. Please try after sometime"); // Indicate an error occurred
            }
        }
    }
}
