using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public interface IuserRegistrationRepo
    {
        public Task<(bool, string)> RegisterUser(UserRegistrationDTO userCred, string OTP);
        public int LoginUser(LoginModel loginCred);
        public Task<bool> ValidateEmail(string email);
       public Task<(bool, string)> ValidateResetId(string email, string resetId);
        public Task<bool> savePasswordInfo(string email, string resetId);
        public Task<(bool, string)> resetPassword(ResetPasswordDto resetPasswordModel);
        public Task<(bool, string)> ValidateRegOTP(string email, string otp);
    }
}
