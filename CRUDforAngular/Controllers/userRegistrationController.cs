using CRUDforAngular.BusinessLayer.CommonService;
using CRUDforAngular.BusinessLayer.DTOs;
using CRUDforAngular.BusinessLayer.Models;

using CRUDforAngular.BusinessLayer.Repos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NETCore.MailKit.Core;
using System.ComponentModel.DataAnnotations;

namespace CRUDforAngular.Controllers
{
    [ApiController]
    public class userRegistrationController : ControllerBase
    {

        private readonly IuserRegistrationRepo _userRegistrationRepo;
        private readonly BusinessLayer.CommonService.EmailService _emailService;
        public userRegistrationController(IuserRegistrationRepo userRegistrationRepo, BusinessLayer.CommonService.EmailService emailService)
        {
            _userRegistrationRepo = userRegistrationRepo;
            _emailService = emailService;

        }

        [HttpPost]
        [Route("api/userRegistration")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userCred)
        {
            string OTP = string.Empty;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Conflict(new Response<List<string>>
                {
                    Success = false,
                    Message = $"Validation failed.{errors.ToString()}",
                    Data = errors
                });
            } 
            OTP =   new Random().Next(0, 999999).ToString("D6"); // Generate a random 6 digit OTP
           
            (bool result, string message) = await  _userRegistrationRepo.RegisterUser(userCred, OTP);
  
            return new JsonResult(new Response<string>
            {
                Success = result,
                Message = message,
                Data = ""
            });

        }

        // validate the OTP and confirm the user registration
        [HttpGet]
        [Route("api/userRegistration/validateRegOTP")]
        public async Task<IActionResult> ValidateOTP([FromQuery] string email, [FromQuery] string otp)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(otp))
                {
                    return BadRequest("Email and OTP cannot be null or empty.");
                }
                var validationResult = await _userRegistrationRepo.ValidateRegOTP(email, otp);

                return Ok(new Response<string>
                {
                    Success = validationResult.Item1,
                    Message = validationResult.Item2,
                    Data = ""
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<string>
                {
                    Success = false,
                    Message = $"An error occurred while processing your request: {ex.Message}",
                    Data = ""
                });
            }
        }

        [HttpPost]
        [Route("api/userRegistration/confirm")]
        public IActionResult Index(int id)
        {
            return new JsonResult(new { message = "User registration confirmed.", userId = id });
        }

        [HttpPost]
        [Route("api/userLogin")]
        public IActionResult Login(LoginModel loginCred)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Conflict(new Response<int>
                {
                    Success = false,
                    Message = $"Validation failed.{errors.ToString()}",
                    Data = -2
                });
            }

            int result = _userRegistrationRepo.LoginUser(loginCred);

            if (result == -1)
            {
                return Ok(new Response<int>
                {
                    Success = false,
                    Message = "User Not Exist",
                    Data = result
                });
            }
            if (result == 0)
            {
                return Ok(new Response<int>
                {
                    Success = false,
                    Message = "Invalid credentials.",
                    Data = result
                });
            }

            return Ok(new Response<int>
            {
                Success = true,
                Message = "success.",
                Data = result
            });
        }


        [HttpGet]
        [Route("api/userPasswordReset")]
        public async Task<IActionResult> resetPasswordforUser([FromQuery] [Required(ErrorMessage ="Email Id is required")]
         [EmailAddress(ErrorMessage ="Entered Email Address is not valid")]string emailId)
        {
            try
            {
                bool dbUdated = false;
                bool emailsent = false;
                if (string.IsNullOrEmpty(emailId))
                {
                    return BadRequest("Email ID cannot be null or empty.");
                }
                var emailValidation = _userRegistrationRepo.ValidateEmail(emailId);

                //if emailValidation.Result is true, send password reset link through email
                if (emailValidation.Result)
                {
                    Guid resetId = Guid.NewGuid();
                    string resetLink = $"https://ruthik-first-project-silk.vercel.app/resetpassword?email={emailId}&resetId={resetId}"; // Generate your reset link here
                                                                                                                                         // Save the resetId and emailId in the database for later validation


                    dbUdated = await _userRegistrationRepo.savePasswordInfo(emailId, resetId.ToString());
                    if (dbUdated)
                    {
                        emailsent = await _emailService.sendPasswordResetMail(emailId, resetLink);
                    }
                    return Ok(new Response<string>
                    {
                        Success = emailsent,
                        Message = emailsent ? "Password reset link sent successfully." : "Error in sending email. try again after sometimes!! ",
                        Data = resetLink
                    });
                }
                else
                {
                    return Ok(new Response<string>
                    {
                        Success = false,
                        Message = $"Email not found. Please use a valid email or <a href='http://localhost:4200/Register' style='color: #007bff; text-decoration: none; font-weight: bold;'>Register</a>.",
                        Data = null
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response<string>
                {
                    Success = false,
                    Message = $"An error occurred while processing your request",
                    Data = null
                });


            }
        }

        [HttpPost]
        [Route("api/validate-resetPswd")]
        public async Task<IActionResult> validateresetPasswordId([FromBody] validateResetPasswordDto resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Conflict(new Response<List<string>>
                {
                    Success = false,
                    Message = $"Validation failed.{errors.ToString()}",
                    Data = errors
                });
            }
            (bool result, string message) = await _userRegistrationRepo.ValidateResetId(resetPasswordModel.email, resetPasswordModel.resetId);

            return Ok(new Response<bool>
            {
                Success = result,
                Message = message,
                Data = result
            });

        }

        //generate resetpassword endpoint
        [HttpPost]
        [Route("api/resetPassword")]
        public async Task<IActionResult> resetPassword([FromBody] ResetPasswordDto resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Conflict(new Response<List<string>>
                {
                    Success = false,
                    Message = $"Validation failed.{errors.ToString()}",
                    Data = errors
                });
            }
            (bool result, string message) = await _userRegistrationRepo.resetPassword(resetPasswordModel);
            
                return Ok(new Response<bool>
                {
                    Success = result,
                    Message = message,
                    Data = result
                });
            

        }
    }
}

