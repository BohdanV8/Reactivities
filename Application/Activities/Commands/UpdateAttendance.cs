using Application.Core;
using Application.Intarfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class UpdateAttendance
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string Id { get; set; }
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly AppDbContext _appDbContext;
            public Handler(IUserAccessor userAccessor, AppDbContext appDbContext)
            {
                _userAccessor = userAccessor;
                _appDbContext = appDbContext;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (!Guid.TryParse(request.Id, out Guid activityId))
                {
                    return Result<Unit>.Failure("Invalid activity id format.", 400);
                }

                Activity? activity = await _appDbContext.Activities
                    .Include(x => x.Attendees)
                    .ThenInclude(x => x.User)
                    .SingleOrDefaultAsync(x => x.Id == activityId, cancellationToken);

                if (activity == null)
                {
                    return Result<Unit>.Failure("Activity not found.", 404);
                }

                User user = await _userAccessor.GetUserAsync();
                ActivityAttendee? attendance = activity.Attendees.FirstOrDefault(x => x.UserId == user.Id);
                bool isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);
                if (attendance != null && isHost)
                {
                    activity.IsCancelled = !activity.IsCancelled;
                }
                else if (attendance != null && !isHost)
                {
                    activity.Attendees.Remove(attendance);
                }
                else if (attendance == null)
                {
                    attendance = new ActivityAttendee
                    {
                        User = user,
                        Activity = activity,
                        IsHost = false
                    };
                    activity.Attendees.Add(attendance);
                }
                bool result = await _appDbContext.SaveChangesAsync(cancellationToken) > 0;
                return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("Failed to update attendance.", 400);
            }
        }
    }
}
