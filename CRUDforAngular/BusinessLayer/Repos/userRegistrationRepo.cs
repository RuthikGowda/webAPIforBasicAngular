using CRUDforAngular.BusinessLayer.CommonService;
using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public class userRegistrationRepo : IuserRegistrationRepo
    {
        private readonly MyDBContext _context;
        private readonly IuserProfileRepo _userProfileRepo;
        private readonly EmailService _emailService;
        private readonly ILogger<userRegistrationRepo> _logger;
        public userRegistrationRepo(MyDBContext context, IuserProfileRepo userProfileRepo,
            EmailService emailService, ILogger<userRegistrationRepo> logger)
        {
            _context = context;
            _userProfileRepo = userProfileRepo;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<(bool, string)> RegisterUser(UserRegistrationDTO userCred, string OTP)
        {
            try
            {
                var data = await _context.UserInfo.AsNoTracking().Select(s => new
                {
                    s.Email,
                    s.isVerified
                }).FirstOrDefaultAsync(u => u.Email.ToLower() == userCred.Email.ToLower());
                
                if (data != null)
                {
                    if (!data.isVerified )
                        await _userProfileRepo.deleteEmpByIDAsync(data.Email);
                    else
                    {
                        return (false, "Email already exists in Database!!");
                    }
                }
                
                var hasher = new PasswordHasher<UserRegistrationDTO>();
                userCred.Password = hasher.HashPassword(userCred, userCred.Password);
                UserInfo user = new UserInfo
                {
                    FirstName = userCred.FirstName,
                    LastName = userCred.LastName, 
                    Email = userCred.Email.ToLower(),
                    UserRegistration = new userRegistration
                    {
                        Email = userCred.Email.ToLower(),
                        Password= userCred.Password,
                        confirmPassword = userCred.confirmPassword,
                        createdDateTime = DateTime.UtcNow,
                        OTP = OTP, 

                    }
                };  
                _context.user.Add(user);
                _context.SaveChanges();
                 await _emailService.sendRegisterOTPMail(userCred.Email.Trim(), OTP);
                return (true, "User Registered Successfully. sent OTP"); // Return the ID of the newly created user
            }
            catch (Exception  ex)
            {
                var errorInfo = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true }));

                // Log the exception (ex) as needed

                return (false, "An error occurred while processing your request."); // Indicate an error occurred
            }
        }

        public int LoginUser(LoginModel loginCred)
        {
            try
            {
                //var Exist = _context.user.Any(u => u.Email == loginCred.Email);
                var Exist = _context.UserInfo.Any(u => u.Email.ToLower() == loginCred.Email.ToLower() && u.isVerified== true);
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
                        user.Password = hasher.HashPassword(resetPasswordModel, resetPasswordModel.Password);
                        user.confirmPassword = resetPasswordModel.confirmPassword; // Assuming you want to set confirmPassword as well
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
                var errorInfo = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true }));

                return (false, "An error occurred while validating the reset ID. Please try after sometime"); // Indicate an error occurred
            }
        }

        public async Task<(bool, string)> ValidateRegOTP(string email, string otp)
        {
            try
            {
                var user = _context.UserInfo.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
                if (user == null)
                {
                    return (false, "User not found."); // User not found
                }
                if (  user.OTP != otp)
                {
                    return (false, "Invalid OTP"); // Invalid OTP
                }
                // OTP is valid, mark the user as verified 
                user.isVerified = true;
                await _context.SaveChangesAsync();

                return (true, "OTP validated successfully");
            }
            catch(Exception ex)
            {
                var errorInfo = new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true }));

                throw;
            }
        }
    }
}
