using CRUDforAngular.BusinessLayer.DTOs;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public interface IuserProfileRepo
    {
        public UserProfileDTO GetUserDataAsync(string emailId);
        public string UpdateUserProfileAsync(UserProfileDTO userProfile);

    }
}
