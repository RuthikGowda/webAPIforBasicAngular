using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CRUDforAngular.BusinessLayer.DTOs.Admin;
using CRUDforAngular.BusinessLayer.Models;
using CRUDforAngular.BusinessLayer.Repos;
using CRUDforAngular.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace CRUDforAngular.BusinessLayer.Repos
{
    public interface IAdminHomePageManage
    {
        public Task<bool> AddCarousel(CarouselDTO carouselDTO, string filePath);
        public Task<IList<carouselBanner>> GetCarouselAsync();
        public Task<bool> DeleteCarouselAsync(int id);
        public Task<bool> AddCategory(ProductCategory productCategory);

        public Task<IList<CategoryItem>> GetProductCategories();

        // Define methods that will be implemented in AdminHomePageManage class
    }

    public class AdminHomePageManage : IAdminHomePageManage
    {
        private readonly IConfiguration _configuration;
        private readonly MyDBContext myDBContext;

        public AdminHomePageManage(IConfiguration configuration, MyDBContext dbContext)
        {
            _configuration = configuration;
            myDBContext = dbContext;
            // Initialize any required services or configurations here
        }
        public async Task<bool> AddCarousel(CarouselDTO carouselDTO, string filePath)
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
                    var carouselEntity = new carouselBanner
                    {
                        // Don't set Id - let EF Core auto-generate it
                        Title = carouselDTO.Title,
                        Description = carouselDTO.Description,
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    myDBContext.carouselBanner.Add(carouselEntity);
                    await myDBContext.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in AddCarousel: {ex.Message}");
                return false;

            }
        }

        

        public async Task<bool> DeleteCarouselAsync(int id)
        {
            try
            {
                int rowsDeleted = await myDBContext.carouselBanner.Where(c => c.Id == id).ExecuteDeleteAsync();
                await myDBContext.SaveChangesAsync();
                return rowsDeleted > 0;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in DeleteCarouselAsync: {ex.Message}");
                return false;
            }
        }

        #region GetCarouselAsync
        public async Task<IList<carouselBanner>> GetCarouselAsync() => await myDBContext.carouselBanner.AsNoTracking().Select(s => new carouselBanner()
        {
            Id = s.Id,
            ImageUrl = s.ImageUrl,
            Title = s.Title,
            Description = s.Description,
            IsActive = s.IsActive,
            CreatedDate = s.CreatedDate
        }).Where(c => c.IsActive)
          .ToListAsync();
        #endregion

        public async Task<bool> AddCategory(ProductCategory productCategory)
        {
            try
            {
                var prodExist = await myDBContext.productCategory
                    .FirstOrDefaultAsync(c => c.categoryId == productCategory.categoryId);

                if (prodExist == null)
                {
                    myDBContext.productCategory.Add(productCategory);
                }
                else
                {
                    prodExist.categoryName = productCategory.categoryName;
                    prodExist.categoryDescription = productCategory.categoryDescription;
                    prodExist.Imageurl = productCategory.Imageurl;
                    myDBContext.productCategory.Update(prodExist);
                }

                return await myDBContext.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error in AddCategory: {ex.Message}");
                return false;
            }
        }
        public async Task<IList<CategoryItem>> GetProductCategories()
        {
            var productCategories = await myDBContext.productCategory.Select(s=> new CategoryItem()
            {
                Id = s.categoryId,
                Title = s.categoryName,
                Description = s.categoryDescription,
                ImageUrl = s.Imageurl
            }).ToListAsync();
            return productCategories;
        }

    }

}
