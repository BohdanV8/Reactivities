using Application.Activities.Entities;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Queries
{
    public class GetActivityDetails
    {
        public class Query : IRequest<Result<ActivityEntity>>
        {
            public Query(Guid id) { this.id = id; }
            public Guid id { get; set; }
        }

        public class Handler (AppDbContext appDbContext, IMapper mapper) : IRequestHandler<Query, Result<ActivityEntity>>
        {
            public async Task<Result<ActivityEntity>> Handle(Query request, CancellationToken cancellationToken)
            {
                ActivityEntity? activity = await appDbContext.Activities.ProjectTo<ActivityEntity>(mapper.ConfigurationProvider).FirstOrDefaultAsync(x => request.id == x.Id, cancellationToken);
                if (activity == null)
                {
                    return Result<ActivityEntity>.Failure("Activity not found", 404);
                }
                else
                {
                    return Result<ActivityEntity>.Success(activity);
                }
            }
        }
    }
}
