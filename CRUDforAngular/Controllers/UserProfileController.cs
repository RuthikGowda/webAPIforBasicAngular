using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Repos;
using Microsoft.AspNetCore.Mvc;

namespace CRUDforAngular.Controllers
{
    [ApiController]
    public class UserProfileController : ControllerBase
    {

        private readonly IuserProfileRepo _userProfileRepo;
        public UserProfileController(IuserProfileRepo userProfileRepo)
        {
            _userProfileRepo = userProfileRepo;
        }
         
        [HttpGet]
        [Route("api/getUserData")]
        public IActionResult getUserdata([FromQuery] string emailId)
        {
            if (string.IsNullOrEmpty(emailId))
            {
                return BadRequest("Email ID cannot be null or empty.");
            }
            UserProfileDTO userProfile = _userProfileRepo.GetUserDataAsync(emailId);

            return Ok();
        }

        [HttpPost]
        [Route("api/updateUserProfile")]
        public IActionResult UpdateUserProfile([FromBody] UserProfileDTO userProfile)
        {
            if (userProfile == null)
            {
                return BadRequest("User profile data cannot be null.");
            }

           string result =  _userProfileRepo.UpdateUserProfileAsync(userProfile);



            // Here you would typically call a service to update the user profile in the database.
            // For now, we will just return a success message.
            return Ok(new { Message = "User profile updated successfully." });

        }

      
    }
}
