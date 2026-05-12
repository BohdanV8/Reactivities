using Application.Core;
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
    public class GetFollowings
    {
        public class Query : IRequest<Result<List<UserProfile>>>
        {
            public required string UserId { get; set; }
            public string Predicate { get; set; } = "followers";
        }

        public class Handler : IRequestHandler<Query, Result<List<UserProfile>>>
        {
            private readonly IMapper _mapper;
            private readonly AppDbContext _appDbContext;
            public Handler(AppDbContext appDbContext, IMapper mapper)
            {
                _appDbContext = appDbContext;
                _mapper = mapper;
            }
            public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profiles = new List<UserProfile>();

                switch (request.Predicate)
                {
                    case "followers":
                        profiles = await _appDbContext.UserFollowings
                            .Where(x => x.TargetId == request.UserId)
                            .Select(u => u.Observer)
                            .ProjectTo<UserProfile>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);
                        break;
                    case "following":
                        profiles = _appDbContext.UserFollowings
                            .Where(x => x.ObserverId == request.UserId)
                            .Select(u => u.Target)
                            .ProjectTo<UserProfile>(_mapper.ConfigurationProvider)
                            .ToList();
                        break;
                    default:
                        return Result<List<UserProfile>>.Failure("Invalid predicate", 400);
                }

                return Result<List<UserProfile>>.Success(profiles);
            }
        }
    }
}
