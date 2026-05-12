using Application.Core;
using Application.Intarfaces;
using Application.Profiles.Entities;
using AutoMapper;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Commands
{
    public class EditProfile : IRequest<Result<Unit>>
    {
        public class Command : IRequest<Result<Unit>>
        {
            public EditProfileEntity profile { get; set; }
            public Command(EditProfileEntity profile)
            {
                this.profile = profile;
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly AppDbContext _appDbContext;
            private readonly IPhotoService _photoService;
            private readonly IMapper _mapper;
            public Handler(AppDbContext appDbContext, IPhotoService photoService, IUserAccessor userAccessor, IMapper mapper)
            {
                this._appDbContext = appDbContext;
                this._photoService = photoService;
                this._mapper = mapper;
                this._userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                User currentUser = await _userAccessor.GetUserAsync();
                currentUser.DisplayName = request.profile.DisplayName ?? currentUser.DisplayName;
                currentUser.Bio = request.profile.Bio ?? currentUser.Bio;
                if (request.profile.File != null)
                {
                    var uploadResult = await _photoService.UploadPhoto(request.profile.File);
                    if (uploadResult == null)
                    {
                        return Result<Unit>.Failure("Failed to upload photo", 400);
                    }
                    Photo photo = new Photo
                    {
                        URL = uploadResult.Url,
                        PublicId = uploadResult.PublicId,
                        UserId = currentUser.Id,
                    };
                    currentUser.ImageUrl = photo.URL;
                    _appDbContext.Photos.Add(photo);
                }

                return await _appDbContext.SaveChangesAsync(cancellationToken) > 0 ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update profile", 400);
            }
        }
    }
}
