using MediatR;
using Domain;
using Microsoft.EntityFrameworkCore;
using Application.Core;
using Application.Intarfaces;
using AutoMapper;
using Application.Activities.Entities;
namespace Application.Activities.Queries
{
    public class GetActivityList
    {
        public class Query() : IRequest<Result<PageList<ActivityEntity>>>
        {
            public required ActivityParams Params { get; set; } = new ActivityParams();
        }

        public class Handler(AppDbContext appDbContext, IUserAccessor _userAccessor, IMapper mapper) : IRequestHandler<Query, Result<PageList<ActivityEntity>>>
        {
            public async Task<Result<PageList<ActivityEntity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var query = appDbContext.Activities
                    .Where(a => a.Date >= request.Params.StartDate)
                    .OrderBy(a => a.Date)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Params.SearchTerm))
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
                var count = await query.CountAsync(cancellationToken);

                List<Activity> activities = await query
                    .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
                    .Take(request.Params.PageSize)
                    .ToListAsync(cancellationToken);

                List<ActivityEntity> mappedActivities = mapper.Map<List<ActivityEntity>>(activities, opt =>
                {
                    opt.Items["currentUserId"] = _userAccessor.getUserId();
                });

                PageList<ActivityEntity> pageList = new PageList<ActivityEntity>(mappedActivities, count, request.Params.PageNumber, request.Params.PageSize);

                return Result<PageList<ActivityEntity>>.Success(pageList);
            }
        }
    }
}
