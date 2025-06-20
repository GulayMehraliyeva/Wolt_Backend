using AutoMapper;
using Azure.Core;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Service.ViewModels.Slider;


namespace Service.Services
{
    public class SliderService : ISliderService
    {
        private readonly ISliderRepository _sliderRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;


        public SliderService(ISliderRepository sliderRepository, 
                             IMapper mapper, 
                             IWebHostEnvironment environment)
        {
            _sliderRepository = sliderRepository;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task CreateAsync(SliderCreateVM request)
        {
            if (request.Image == null)
                throw new Exception("Image is required");

            if (!request.Image.ContentType.Contains("image/"))
                throw new Exception("Only image files are allowed");

            var slider = _mapper.Map<Slider>(request);

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

            slider.Image = fileName;

            await _sliderRepository.CreateAsync(slider);
        }




        public async Task DeleteAsync(int id)
        {
            var slider = await _sliderRepository.GetByIdAsync(id);
            if (slider == null)
                throw new Exception("Slider not found");

            string imagePath = Path.Combine(_environment.WebRootPath, "assets/images", slider.Image);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            await _sliderRepository.DeleteAsync(slider);
        }

        public async Task<IEnumerable<SliderVM>> GetAllAsync()
        {
            return _mapper.Map<IEnumerable<SliderVM>>(await _sliderRepository.GetAllAsync());
        }

        public async Task<SliderVM> GetByIdAsync(int id)
        {
            var slider = await _sliderRepository.GetByIdAsync(id);
            if (slider == null) return null;

            return _mapper.Map<SliderVM>(slider);
        }

        public async Task EditAsync(int id, SliderEditVM editVm)
        {
            var existingSlider = await _sliderRepository.GetByIdAsync(id);
            if (existingSlider == null)
                throw new Exception("Slider not found");

            _mapper.Map(editVm, existingSlider);

            if (editVm.UploadImage != null)
            {
                if (!editVm.UploadImage.ContentType.Contains("image/"))
                    throw new Exception("Input type must be only image");

                string oldImagePath = Path.Combine(_environment.WebRootPath, "assets/images", existingSlider.Image);
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

                existingSlider.Image = fileName; 
            }

            await _sliderRepository.UpdateAsync(existingSlider);
        }


    }
}
