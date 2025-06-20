using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
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
        public  UserProfileDTO  GetUserDataAsync(string emailId)
        {
            throw new NotImplementedException();
        }

        public string UpdateUserProfileAsync(UserProfileDTO userProfile)
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
                .Where(u => !u.Email.Equals(userProfile.Email, StringComparison.OrdinalIgnoreCase))
                .SelectMany(u => u.Phone)
                .FirstOrDefault(p => phoneNumbers.Contains(p.PhoneNumber));

            if (duplicatePhone != null)
            {
                return  "Phone Number Duplicate"; ;
            }

            if (userProfile != null)
            {
                var userInfo = _context.user.FirstOrDefault(u => u.Id == userProfile.Id);
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
            }
            return "success";
        }

        UserProfileDTO IuserProfileRepo.GetUserDataAsync(string emailId)
        {

            var user = _context.user
                .Include(u => u.Phone)
                .Include(u => u.Address)
                .FirstOrDefault(u => u.Email.Equals(emailId, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                return null; // or throw an exception
            }
            return new UserProfileDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Phone = user.Phone.ToList(),
                Address = user.Address.ToList()
            };
        }

        
        }
}
