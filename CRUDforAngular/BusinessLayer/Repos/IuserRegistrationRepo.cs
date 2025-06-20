using CRUDforAngular.BusinessLayer.Models;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public interface IuserRegistrationRepo
    {
        public int RegisterUser(userRegistration userCred);
        public int LoginUser(LoginModel loginCred);
        public Task<bool> ValidateEmail(string email);
    }
}
