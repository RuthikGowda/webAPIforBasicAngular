using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public class userProfileRepo : IuserProfileRepo
    {
        private readonly MyDBContext _context;
        public userProfileRepo(MyDBContext context)
        {
            _context = context;
        }
        public async Task<UserProfileDTO?> GetUserDataAsync(string emailId)
        {
            return await _context.user
                .Include(u => u.Phone)
                .Include(u => u.Address)
                .Where(u => u.Email.ToUpper().Equals(emailId.ToUpper()))
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    Phone = u.Phone.ToList(),
                    Address = u.Address.ToList()
                })
                .FirstOrDefaultAsync();
        }


        public async Task<IList<UserProfileDTO>> GetAllEmployeeDataAsync()
        {
            var userProfiles = await _context.user
                .Include(u => u.Phone)
                .Include(u => u.Address)
                .Select(u => new UserProfileDTO
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DateOfBirth = u.DateOfBirth,
                    Phone = u.Phone.ToList(),
                    Address = u.Address.ToList(),
                    
                })
                .ToListAsync();

            return userProfiles;
        }


        public async Task<string> UpdateUserProfileAsync(UserProfileDTO userProfile)
        {
            //// Check if phone number is duplicate with existing user  
            //foreach (var phone in userProfile.Phone)
            //{
            //    var existingPhoneEntry = _context.UserPhones
            //        .FirstOrDefault(p => p.PhoneNumber == phone.PhoneNumber && p.UserInfoId == userProfile.Id);
            //    if (existingPhoneEntry != null)
            //    {
            //        return "Phone Number Duplicate";
            //    }
            //}

            // Check if PhoneNumber is duplicate with other users  
            var phoneNumbers = userProfile.Phone.Select(p => p.PhoneNumber).ToHashSet<long>();
            var duplicatePhone = _context.user
               .AsEnumerable() // Switch to client-side evaluation  
               .Where(u => !u.Email.Equals(userProfile.Email, StringComparison.OrdinalIgnoreCase))
               .SelectMany(u => u.Phone)
               .FirstOrDefault(p => phoneNumbers.Contains(p.PhoneNumber));

            if (duplicatePhone != null)
            {
                return "Phone Number Duplicate"; ;
            }

            if (userProfile != null)
            {
                var userInfo = await _context.user.FirstOrDefaultAsync(u => u.Id == userProfile.Id);
                if (userInfo != null)
                {
                    userInfo.FirstName = userProfile.FirstName;
                    userInfo.LastName = userProfile.LastName;
                    userInfo.Email = userProfile.Email;
                    userInfo.DateOfBirth = userProfile.DateOfBirth;
                    userInfo.Phone = userProfile.Phone;
                    userInfo.Address = userProfile.Address;

                    _context.user.Update(userInfo);

                }
                else
                {
                    var loginExist = _context.UserInfo.FirstOrDefault(u => u.Email == userProfile.Email);
                    if (loginExist is null)
                    {
                        _context.user.Add(new UserInfo
                        {
                            FirstName = userProfile.FirstName,
                            LastName = userProfile.LastName,
                            Email = userProfile.Email,
                            DateOfBirth = userProfile.DateOfBirth,
                            Phone = userProfile.Phone,
                            UserRegistration = new userRegistration
                            {
                                Email = userProfile.Email,
                            },
                            // Assuming Id is the UserRegistrationId
                            Address = userProfile.Address,
                        });
                    }
                    else
                    {
                        // Check if UserRegistration exists  
                        var existingRegistration = await _context.UserInfo.AsNoTracking().FirstOrDefaultAsync(u => u.Email == userProfile.Email);

                        if (existingRegistration != null)
                        {
                            // Map UserRegistrationId to UserInfo  
                            var newUser = new UserInfo
                            {
                                FirstName = userProfile.FirstName,
                                LastName = userProfile.LastName,
                                Email = userProfile.Email,
                                DateOfBirth = userProfile.DateOfBirth,
                                Phone = userProfile.Phone,
                                Address = userProfile.Address,
                                UserRegistrationId = existingRegistration.Id // Map existing UserRegistrationId  
                            };

                            _context.user.Add(newUser);

                        }
                    }
                }
                _context.SaveChanges();
                return "success";
            }
            return "User not found";
        }
         public async Task<bool> deleteEmpByIDAsync(int id)
        {
            try
            {
                var exist = await _context.user.FirstOrDefaultAsync(u => u.Id == id);

                if (exist != null)
                {
                    await _context.user.Where(u => u.Id == id).ExecuteDeleteAsync();
                    await _context.UserInfo.Where(u => u.Email == exist.Email).ExecuteDeleteAsync();
                    _context.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception)
            {
                throw;
            }
        }

        

     }
}
