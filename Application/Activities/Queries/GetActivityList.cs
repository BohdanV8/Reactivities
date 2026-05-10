using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Activities.Entities;
namespace Application.Activities.Queries
{
    public class GetActivityList
    {
        public class Query() : IRequest<Result<List<ActivityEntity>>>
        {
        }

        public class Handler(AppDbContext appDbContext, IMapper mapper) : IRequestHandler<Query, Result<List<ActivityEntity>>>
        {
            public async Task<Result<List<ActivityEntity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = await appDbContext.Activities.ProjectTo<ActivityEntity>(mapper.ConfigurationProvider).ToListAsync(cancellationToken);
                return Result<List<ActivityEntity>>.Success(activities);
            }
        }
    }
}
