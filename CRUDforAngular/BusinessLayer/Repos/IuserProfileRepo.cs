using CRUDforAngular.BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public interface IuserProfileRepo
    {
       
        public   Task<UserProfileDTO?> GetUserDataAsync(string emailId);
         
        public   Task<IList<UserProfileDTO>> GetAllEmployeeDataAsync();
        public Task<string> UpdateUserProfileAsync(UserProfileDTO userProfile);

       // public Task<Boolean> deleteEmpByIDAsync(int id);
        public   Task<bool> deleteEmpByIDAsync(int id);

    }
}
