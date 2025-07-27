using CRUDforAngular.BusinessLayer.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using static System.Net.WebRequestMethods;

namespace CRUDforAngular.BusinessLayer.CommonService
{
    public class EmailService
    {
        private readonly smtpOptions _smtpOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<smtpOptions> smtpOptions, ILogger<EmailService> logger)
        {
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
        }
        public async Task sendRegisterOTPMail(string email, string otp)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.Body = $"<div style=\"font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2\">\r\n  <div style=\"margin:50px auto;width:70%;padding:20px 0\">\r\n   " +
                    $" <div style=\"border-bottom:1px solid #eee\">\r\n      <a href=\"\" style=\"font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600\">My Angular Basic</a>\r\n    </div>\r\n    " +
                    $"<p style=\"font-size:1.1em\">Hi, {email.Replace("@gmail.com", "")}</p>\r\n    <p>Thank you for choosing Your Brand. Use the following OTP to complete your Sign Up procedures. OTP is valid for 5 minutes</p>\r\n    <h2 style=\"background: #00466a;margin: 0 auto;width: max-content;padding: " +
                    $"0 10px;color: #fff;border-radius: 4px;\">{otp}</h2>\r\n    <p style=\"font-size:0.9em;\">Regards,<br />Ruthik y</p>\r\n    <hr style=\"border:none;border-top:1px solid #eee\" />\r\n    <div style=\"float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300\">\r\n    " +
                    $"  <p>Your Brand Inc</p>\r\n      <p>1600 Amphitheatre Parkway</p>\r\n      <p>California</p>\r\n    </div>\r\n  </div>\r\n</div>";

                mailMessage.Subject = "Your OTP for Registration";
                mailMessage.To.Add(email);
                mailMessage.From = new MailAddress(_smtpOptions.username,_smtpOptions.displayName);
                mailMessage.IsBodyHtml = true; // Set to true to send HTML content

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _smtpOptions.Host;
                smtp.Port = _smtpOptions.Port;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(_smtpOptions.username, _smtpOptions.appPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mailMessage);




                Console.WriteLine($"Sending OTP {otp} to {email}");

                  // Return 1 to indicate success
            }
            catch (Exception ex)
            {
                var errorInfo = new {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(System.Text.Json.JsonSerializer.Serialize(errorInfo, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                throw;
                 // Return -1 to indicate failure
            }
        }

        public async Task<bool> sendPasswordResetMail(string emailId, string resetLink)
        {
            try
            {
                //geneare a good looking email template for password reset
                MailMessage mailMessage = new MailMessage();
                mailMessage.Body = $@"<div style=""font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2"">
               <div style=""margin:50px auto;width:70%;padding:20px 0"">
                   <div style=""border-bottom:1px solid #eee"">
                       <a href="""" style=""font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600"">My Angular Basic</a>
                   </div>
                   <p style=""font-size:1.1em"">Hi, {emailId.Replace("@gmail.com", "")}</p>
                   <p>Please use the below link to reset your password. This link is valid for 10 minutes.<br/></p>
                   <h4 style=""background: #00466a; margin: 0 auto; width: max-content; padding: 0 15px; color: #fff; border-radius: 4px;"">
                       <a href=""{resetLink}"" style=""color: #fff; text-decoration: none;"">Reset Password</a>
                   </h4>
                   <p style=""font-size:0.9em;"">Regards,<br />Ruthik y</p>
                   <hr style=""border:none;border-top:1px solid #eee"" />
                   <div style=""float:right;padding:8px 0;color:#aaa;font-size:0.8em;line-height:1;font-weight:300"">
                       <p>Your Brand Inc</p>
                       <p>1600 Amphitheatre Parkway</p>
                       <p>California</p>
                   </div>
               </div>
            </div>";

                mailMessage.Subject = "Password Reset Request";
                mailMessage.To.Add(emailId);
                mailMessage.From = new MailAddress(_smtpOptions.username, _smtpOptions.displayName);
                // add company name to show in email in From field
                 mailMessage.IsBodyHtml = true; // Set to true to send HTML content

                SmtpClient smtp = new SmtpClient();
                smtp.Host = _smtpOptions.Host;
                smtp.Port = _smtpOptions.Port;

                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(_smtpOptions.username, _smtpOptions.appPassword);

                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mailMessage);


                Console.WriteLine($"Sent mail {emailId}  to {resetLink}");

                return true; // Return true to indicate success
            }
            catch (Exception ex)
            {
                var errorInfo = new {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message,
                    Source = ex.Source
                };
                _logger.LogError(System.Text.Json.JsonSerializer.Serialize(errorInfo, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                return false; // Return false to indicate failure

            }
        }
    }
}
