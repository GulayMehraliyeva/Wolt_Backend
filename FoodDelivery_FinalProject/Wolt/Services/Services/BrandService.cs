using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Brand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        public BrandService(IBrandRepository brandRepository, IMapper mapper, IWebHostEnvironment environment)
        {
            _brandRepository = brandRepository;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<IEnumerable<BrandVM>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BrandVM>>(brands);
        }

        public async Task<BrandVM> GetByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            return brand == null ? null : _mapper.Map<BrandVM>(brand);
        }

        public async Task CreateAsync(BrandCreateVM request)
        {
            if (request.Image == null)
                throw new Exception("Image is required");

            if (!request.Image.ContentType.Contains("image/"))
                throw new Exception("Only image files are allowed");

            var brand = _mapper.Map<Brand>(request);

            string fileName = Guid.NewGuid().ToString() + "-" + request.Image.FileName;
            string folderPath = Path.Combine(_environment.WebRootPath, "assets/images/");
            string filePath = Path.Combine(folderPath, fileName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                await request.Image.CopyToAsync(stream);
            }

            brand.Image = fileName;

            await _brandRepository.CreateAsync(brand);
        }

        public async Task EditAsync(int id, BrandEditVM editVm)
        {
            var existingBrand = await _brandRepository.GetByIdAsync(id);
            if (existingBrand == null)
                throw new Exception("Brand not found");

            _mapper.Map(editVm, existingBrand);

            if (editVm.UploadImage != null)
            {
                if (!editVm.UploadImage.ContentType.Contains("image/"))
                    throw new Exception("Only image files are allowed");

                string oldImagePath = Path.Combine(_environment.WebRootPath, "assets/images", existingBrand.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                string fileName = Guid.NewGuid().ToString() + "-" + editVm.UploadImage.FileName;
                string folderPath = Path.Combine(_environment.WebRootPath, "assets/images");
                string newImagePath = Path.Combine(folderPath, fileName);

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                using (FileStream stream = new FileStream(newImagePath, FileMode.Create))
                {
                    await editVm.UploadImage.CopyToAsync(stream);
                }

                existingBrand.Image = fileName;
            }

            await _brandRepository.UpdateAsync(existingBrand);
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null)
                throw new Exception("Brand not found");

            string imagePath = Path.Combine(_environment.WebRootPath, "assets/images", brand.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            await _brandRepository.DeleteAsync(brand);
        }
    }

}
