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
using Application.Intarfaces;
namespace Application.Activities.Queries
{
    public class GetActivityList
    {
        public class Query() : IRequest<Result<PageList<ActivityEntity>>>
        {
            public required ActivityParams Params { get; set; } = new ActivityParams();
        }

        public class Handler(AppDbContext appDbContext, IMapper mapper, IUserAccessor _userAccessor) : IRequestHandler<Query, Result<PageList<ActivityEntity>>>
        {
            public async Task<Result<PageList<ActivityEntity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = appDbContext.Activities.OrderBy(a => a.Date)
                    .Where(a => a.Date >= request.Params.StartDate)
                    .AsQueryable();

                if(!string.IsNullOrEmpty(request.Params.SearchTerm))
                {
                    string? search = request.Params.SearchTerm.ToLower();
                    query = query.Where(a => a.Title.ToLower().Contains(search) || a.City.ToLower().Contains(search) || a.Venue.ToLower().Contains(search));
                }

                if (request.Params.EndDate.HasValue)
                {
                    query = query.Where(a => a.Date <= request.Params.EndDate.Value);
                }
                if (!string.IsNullOrEmpty(request.Params.Filter))
                {
                    query = request.Params.Filter switch
                    {
                        "isGoing" => query.Where(a => a.Attendees.Any(u => u.UserId == _userAccessor.getUserId())),
                        "isHost" => query.Where(a => a.Attendees.Any(a => a.IsHost && a.UserId == _userAccessor.getUserId())),
                        _ => query
                    };
                }
                var projectedActivities = query.ProjectTo<ActivityEntity>(mapper.ConfigurationProvider, new { currentUserId = _userAccessor.getUserId()});
                List<ActivityEntity> activities = await projectedActivities.Take(request.Params.PageSize + 1).ToListAsync(cancellationToken);
                DateTime? nextCursor = null;
                if(activities.Count > request.Params.PageSize)
                {
                    nextCursor = activities.Last().Date;
                    activities.RemoveAt(activities.Count - 1);
                }
                return Result<PageList<ActivityEntity>>.Success(
                    await PageList<ActivityEntity>.CreateAsync(projectedActivities, request.Params.PageNumber, request.Params.PageSize)
                );
            }
        }
    }
}
