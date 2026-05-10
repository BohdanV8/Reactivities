using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Profiles
{
    public class GetProfilePhotos
    {
        public class Query : IRequest<Result<List<Photo>>>
        {
            public required string UserId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<Photo>>>
        {
            private readonly AppDbContext _appDbContext;
            public Handler(AppDbContext appDbContext)
            {
                _appDbContext = appDbContext;
            }
            public async Task<Result<List<Photo>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var photos = await _appDbContext.Users.Where(x => x.Id == request.UserId).SelectMany(x => x.Photos).ToListAsync(cancellationToken);
                return Result<List<Photo>>.Success(photos);
            }
        }
    }
}
