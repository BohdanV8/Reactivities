using Application.Core;
using Application.Intarfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Commands
{
    public class AddPhoto
    {
        public class Command : IRequest<Result<Photo>>
        {
            public required IFormFile File { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Photo>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly AppDbContext _appDbContext;
            private readonly IPhotoService _photoService;
            public Handler(IUserAccessor userAccessor, AppDbContext appDbContext, IPhotoService photoService)
            {
                _userAccessor = userAccessor;
                _appDbContext = appDbContext;
                _photoService = photoService;
            }
            public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
            {
                var uploadResult = await _photoService.UploadPhoto(request.File);
                if (uploadResult == null)
                {
                    return Result<Photo>.Failure("Failed to upload photo", 400);
                }
                User user = await _userAccessor.GetUserAsync();
                var photo = new Photo
                {
                    URL = uploadResult.Url,
                    PublicId = uploadResult.PublicId,
                    UserId = user.Id,
                };
                if (user.ImageUrl == "")
                {
                    user.ImageUrl = photo.URL;
                }
                _appDbContext.Photos.Add(photo);
                var result = await _appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                {
                    return Result<Photo>.Failure("Failed to add photo to database", 400);
                }
                return Result<Photo>.Success(photo);
            }
        }
    }
}
