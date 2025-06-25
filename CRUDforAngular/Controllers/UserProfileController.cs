using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

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
        public  async Task<IActionResult> getUserdata([FromQuery] string emailId)
        {
            if (string.IsNullOrEmpty(emailId))
            {
                return BadRequest("Email ID cannot be null or empty.");
            }
            UserProfileDTO? userProfile = await _userProfileRepo.GetUserDataAsync(emailId);

            return Ok();
        }

        [HttpPost]
        [Route("api/updateUserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UserProfileDTO userProfile)
        {
            if (userProfile == null)
            {
                return BadRequest("User profile data cannot be null.");
            }

           string result = await _userProfileRepo.UpdateUserProfileAsync(userProfile);
 
            // Here you would typically call a service to update the user profile in the database.
            // For now, we will just return a success message.
            return Ok(new { Message = "User profile updated successfully." });

        }

        [Route("api/getAllEmployeeData")]
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeDataAsync()
        {


            var userProfiles = await _userProfileRepo.GetAllEmployeeDataAsync();
            return Ok(userProfiles);
        }

        [HttpGet]
        [Route("api/deleteByID")]
        public async Task<IActionResult> deleteEmpByID([FromQuery] int id)
        {
            try
            {
                bool deleted = false;
                if (id == 0)
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Message = "deletion invalid!",
                        Data = "deletion failed!"
                    });
                if (id > 0)
                    deleted = await _userProfileRepo.deleteEmpByIDAsync(id);

                return Ok(new Response<string>
                {
                    Success = deleted,
                    Message = deleted ? "deleted succefully" : "user not exist!",
                    Data = deleted ? "deleted successfully" : "user not exist!"
                });
            }
            catch (Exception ex)
            {
                return   Ok(new Response<string>
                {
                    Success = false,
                    Message = "server error occured!",
                    Data = "deleted failed!"
                });
            }
        }


    }
}
