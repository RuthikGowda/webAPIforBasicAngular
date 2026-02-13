using CRUDforAngular.BusinessLayer.CommonService;
using CRUDforAngular.BusinessLayer.DTOs.Admin;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CRUDforAngular.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManageHomeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IAdminHomePageManage _adminHomePageManage;
        private readonly IOpenAIservice _openAIservice;
        private readonly ManageCloudinary _manageCloudinary;
        public ManageHomeController(IWebHostEnvironment WebHostEnvironment, IAdminHomePageManage AdminHomePageManage,
            IOpenAIservice OpenAIservice,ManageCloudinary manageCloudinary)
        {
            _env = WebHostEnvironment;
            _adminHomePageManage = AdminHomePageManage;
            _openAIservice = OpenAIservice;
            _manageCloudinary = manageCloudinary;
        }

        [HttpPost]
        [Route("AddCarousel")]
        public async Task<IActionResult> AddCarousel([FromForm] CarouselDTO carouselDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //store file in
            try
            {
                if (carouselDTO.Image != null && carouselDTO.Image.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + carouselDTO.Image.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Ensure the directory exists
                    Directory.CreateDirectory(uploadsFolder);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await carouselDTO.Image.CopyToAsync(fileStream);
                    }

                    bool result = await _adminHomePageManage.AddCarousel(carouselDTO, filePath);



                    return Ok(new Response<string>
                    {
                        Message = result ? "Banner added successfully" : "Failed to add banner",
                        Success = result,
                        Data = ""

                    });
                }

                return BadRequest("No image was uploaded.");
            }
            catch (Exception ex)
            {
                return Ok(new Response<string>
                {
                    Message = ex.Message,
                    Success = false,
                    Data = ""

                });
            }


        }

        [HttpGet]
        [Route("GetCarousel")]
        public async Task<IActionResult> GetCarousel()
        {
            try
            {
                var carouselData = await _adminHomePageManage.GetCarouselAsync();

                return Ok(new Response<IEnumerable<carouselBanner>>
                {
                    Message = "Carousel data retrieved successfully.",
                    Success = true,
                    Data = carouselData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response<string>
                {
                    Message = ex.Message,
                    Success = false,
                    Data = ""
                });
            }
        }

        [HttpGet]
        [Route("DeleteCarousel/{id}")]
        public async Task<IActionResult> DeleteCarousel(int id)
        {

           bool isDeleted = await _adminHomePageManage.DeleteCarouselAsync(id);
            return Ok(new Response<string>
            {
                Message = isDeleted ? "Deleted Successfully!" : "Failed! Try again.",
                Success = isDeleted,
                Data = ""
            });
        }

        [HttpGet]
        [Route("OpenAI")]
        public async Task<IActionResult> OpenAI(string prompt)
        {

            var reponse = await _openAIservice.GetOpenAIResponse(prompt);
            return Ok(new Response<string>
            {
                Message = "OpenAI response retrieved successfully.",
                Success = true,
                Data = reponse
            }); 

        }

        [HttpPost]
        [Route("DeepSeekAI")]
        public async Task<IActionResult> DeepSeekAI(AiGenerateRequest aiGenerateRequest)
        {

            var reponse = await _openAIservice.GetResponseAsync(aiGenerateRequest);
            return Ok(new Response<string>
            {
                Message = "OpenAI response retrieved successfully.",
                Success = true,
                Data = reponse
            });

        }


        [HttpPost]
        [Route("AddCategory")]
        public async Task<IActionResult> AddCategory([FromForm] CategoryDTO categoryDTO)
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
            if(categoryDTO.Image is  null || categoryDTO.Image.Length <= 0)
                return BadRequest("No image was uploaded.");

            string imageExtension = Path.GetExtension(categoryDTO.Image.FileName).ToLower();
            if (imageExtension != ".png" && imageExtension != ".jpg" && imageExtension != ".jpeg")
            {
                 return Ok(new Response<string>
                {
                    Message = "Invalid image format. Only .png, .jpg, and .jpeg are allowed.",
                    Success = false,
                    Data = ""

                });
            }
             
            string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
            string fileName = $"{Guid.NewGuid().ToString()}_{categoryDTO.Image.FileName}";
            string filePath = Path.Combine(uploadsFolder, fileName);

            Directory.CreateDirectory(uploadsFolder);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await categoryDTO.Image.CopyToAsync(fileStream);
            }

          string UploadedURL = await   _manageCloudinary.UploadImageTOCloudinay(filePath);

            ProductCategory productCategory = new ProductCategory()
            {
                categoryId = categoryDTO.Id,
                categoryName = categoryDTO.Title,
                categoryDescription = categoryDTO.Description,
                Imageurl = UploadedURL
            };

          bool CategoryUploaded = await  _adminHomePageManage.AddCategory(productCategory);


            return Ok(new Response<string>
            {
                Message = CategoryUploaded ? "Category added successfully" : "Failed to add Category",
                Success = CategoryUploaded,
                Data = ""

            });
        }

        [HttpGet]
        [Route("GetCategory")]
        public async Task<IActionResult> getCategory()
        {
            
                var data = await _adminHomePageManage.GetProductCategories();
            return Ok(new Response<IEnumerable<CategoryItem>>
            {
                Message = "Carousel data retrieved successfully.",
                Success = true,
                Data = data
            });

            
             
        }
    }
}
