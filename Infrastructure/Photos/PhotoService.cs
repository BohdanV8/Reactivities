using Application.Intarfaces;
using Application.Profiles;
using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using System;
using System.Collections.Generic;
using System.Text;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }
        public async Task<string> DeletePhoto(string publicId)
        {
            DeletionParams deletionParams = new DeletionParams(publicId);

            DeletionResult result = await _cloudinary.DestroyAsync(deletionParams);
            if(result.Error != null) throw new Exception(result.Error.Message);
            return result.Result;
        }

        public async Task<PhotoUploadResult?> UploadPhoto(IFormFile file)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill"),
                    Folder = "Reactivities2026"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.Error != null) throw new Exception(uploadResult.Error.Message);
                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };
            }

            return null;
        }
    }
}
