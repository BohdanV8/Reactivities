using Application.Core;
using Application.Intarfaces;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Commands
{
    public class SetMainPhoto
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string PhotoId { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _appDbContext;
            private readonly IUserAccessor _userAccessor;
            private readonly IPhotoService _photoService;
            public Handler(AppDbContext appDbContext, IUserAccessor userAccessor, IPhotoService photoService)
            {
                _appDbContext = appDbContext;
                _userAccessor = userAccessor;
                _photoService = photoService;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                User user = await _userAccessor.GetUserWithPhotosAsync();
                Photo? photo = user.Photos.FirstOrDefault(x => x.Id == request.PhotoId);
                if (photo == null) return Result<Unit>.Failure("Photo not found", 404);
                user.ImageUrl = photo.URL;
                var result = await _appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<Unit>.Failure("Failed to set main photo", 500);
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
