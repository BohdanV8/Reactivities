using Application.Core;
using Application.Intarfaces;
using Application.Profiles.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Profiles.Queries
{
    public class GetProfile
    {
        public class Query : IRequest<Result<UserProfile>>
        {
            public required string UserId { get; set; }
        }
        public class Handler : IRequestHandler<Query, Result<UserProfile>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly AppDbContext _appDbContext;
            private readonly IMapper _mapper;
            public Handler(IUserAccessor userAccessor, AppDbContext appDbContext, IMapper mapper)
            {
                this._userAccessor = userAccessor;
                this._appDbContext = appDbContext;
                this._mapper = mapper;
            }
            public async Task<Result<UserProfile>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profile = await _appDbContext.Users.ProjectTo<UserProfile>(_mapper.ConfigurationProvider)
                    .SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

                return profile == null ? Result<UserProfile>.Failure("User not found", 404) : Result<UserProfile>.Success(profile);
            }
        }
    }
}
