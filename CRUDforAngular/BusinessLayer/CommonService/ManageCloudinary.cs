using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace CRUDforAngular.BusinessLayer.CommonService
{
    public class ManageCloudinary
    {
        private readonly IConfiguration _configuration;

        public ManageCloudinary( IConfiguration configuration) { 
            _configuration = configuration;
        }
        public async Task<string> UploadImageTOCloudinay(string filePath)
        {
            try
            {
                Account account = new Account(
                 _configuration["cloudinaryKey:cloudName"],
                 _configuration["cloudinaryKey:apiKey"],
                 _configuration["cloudinaryKey:apiSecret"]);

                Cloudinary cloudinary = new Cloudinary(account);
                cloudinary.Api.Secure = true;
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(filePath),
                    Folder = "banner"
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams);
                File.Delete(filePath); // Clean up the local file after upload
                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return uploadResult.SecureUrl.ToString();
                }
                return "Error";

            }
            catch (Exception ex)
            {
                return "Error";

            }
        }
    }
}
