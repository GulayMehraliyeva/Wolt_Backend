using Domain.Entities;
using Repository.Repositories.Interfaces;
using Service.Services.Interfaces;
using Service.ViewModels.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class SettingService : ISettingService
    {
        private readonly IBaseRepository<Setting> _settingRepo;

        public SettingService(IBaseRepository<Setting> settingRepo)
        {
            _settingRepo = settingRepo;
        }

        public async Task<IEnumerable<SettingVM>> GetAllAsync()
        {
            var settings = await _settingRepo.GetAllAsync();
            return settings.Select(s => new SettingVM
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                Type = s.Type,
            });
        }

        public async Task<SettingEditVM> GetByIdAsync(int id)
        {
            var setting = await _settingRepo.GetByIdAsync(id);
            if (setting == null) return null;

            return new SettingEditVM
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Type = setting.Type,
            };
        }

        public async Task CreateAsync(SettingCreateVM vm)
        {
            var allSettings = await _settingRepo.GetAllAsync();

            bool keyExists = allSettings.Any(s => s.Key.Trim().ToLower() == vm.Key.Trim().ToLower());
            if (keyExists)
                throw new Exception("A setting with the same key already exists.");

            string value = vm.Type == "Text" ? vm.Value : null;

            if (vm.Type == "Image" && vm.Image != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(vm.Image.FileName);
                string path = Path.Combine("wwwroot/assets/images", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await vm.Image.CopyToAsync(stream);
                }

                value = "/assets/images/" + fileName;
            }

            var setting = new Setting
            {
                Key = vm.Key,
                Value = value,
                Type = vm.Type
            };

            await _settingRepo.CreateAsync(setting);
        }



        public async Task UpdateAsync(SettingEditVM vm)
        {
            var setting = await _settingRepo.GetByIdAsync(vm.Id);
            if (setting == null) return;

            var allSettings = await _settingRepo.GetAllAsync();

            bool keyExists = allSettings.Any(s => s.Id != vm.Id && s.Key.Trim().ToLower() == vm.Key.Trim().ToLower());
            if (keyExists)
                throw new Exception("A setting with the same key already exists.");

            setting.Key = vm.Key;
            setting.Type = vm.Type;

            if (vm.Type == "Text")
            {
                setting.Value = vm.Value;
            }
            else if (vm.Type == "Image")
            {
                if (vm.Image != null)
                {
                    if (!string.IsNullOrWhiteSpace(setting.Value))
                    {
                        var oldPath = Path.Combine("wwwroot", setting.Value.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    string fileName = Guid.NewGuid() + Path.GetExtension(vm.Image.FileName);
                    string filePath = Path.Combine("wwwroot/assets/images", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await vm.Image.CopyToAsync(stream);
                    }

                    setting.Value = "/assets/images/" + fileName;
                }
            }

            await _settingRepo.UpdateAsync(setting);
        }

        public async Task DeleteAsync(int id)
        {
            var setting = await _settingRepo.GetByIdAsync(id);
            if (setting == null) return;

            if (setting.Type == "Image" && !string.IsNullOrWhiteSpace(setting.Value))
            {
                var filePath = Path.Combine("wwwroot", setting.Value.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _settingRepo.DeleteAsync(setting);
        }

    }
}
