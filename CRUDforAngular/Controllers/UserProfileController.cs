using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Text.Json;

namespace CRUDforAngular.Controllers
{
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {

        private readonly IuserProfileRepo _userProfileRepo;
        private readonly ILogger<UserProfileController> _logger;
        public UserProfileController(IuserProfileRepo userProfileRepo, ILogger<UserProfileController> logger)
        {
            _userProfileRepo = userProfileRepo;
            _logger = logger;
        }

        [HttpGet]
        [Route("api/getUserData")]
        public  async Task<IActionResult> getUserdata([FromQuery] string emailId)
        {
            try
            {
                _logger.LogInformation("Fetching user data for email: {EmailId}", emailId);
                if (string.IsNullOrEmpty(emailId))
                {
                    return BadRequest("Email ID cannot be null or empty.");
                }
                UserProfileDTO? userProfile = await _userProfileRepo.GetUserDataAsync(emailId);
                // convert userProfile to JSON format and log it
                _logger.LogInformation("User profile data: {0}", JsonSerializer.Serialize(userProfile, new JsonSerializerOptions { WriteIndented = true }));
                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                var errorInfo = new {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true }));
                return StatusCode(500, "Some internal server error occurred, try again later.");
            }
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
        [Route("api/deleteByEmail")]
        public async Task<IActionResult> deleteEmpByID([FromQuery] string email)
        {
            try
            {
                bool deleted = false;
                if (email is null)
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Message = "deletion invalid!",
                        Data = "deletion failed!"
                    });
                deleted = await _userProfileRepo.deleteEmpByIDAsync(email);
                return Ok(new Response<string>
                {
                    Success = deleted,
                    Message = deleted ? "deleted succefully" : "user not exist!",
                    Data = deleted ? "deleted successfully" : "user not exist!"
                });
            }
            catch (Exception ex)
            {
                var errorInfo = new {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(JsonSerializer.Serialize(errorInfo, new JsonSerializerOptions { WriteIndented = true }));
                return Ok(new Response<string>
                {
                    Success = false,
                    Message = "server error occured!",
                    Data = "deleted failed!"
                });
            }
        }


        [HttpGet]
        [Route("api/textExcpetion")]
        public IActionResult throwError()
        {
            throw new NotImplementedException();
           
        }
    }
}
