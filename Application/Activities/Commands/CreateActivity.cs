using Application.Activities.DTOs;
using Application.Core;
using Application.Intarfaces;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            public required CreateActivityEntity Activity { get; set; }
        }

        public class Handler(AppDbContext appDbContext, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userAccessor.GetUserAsync();
                Activity activity = mapper.Map<Activity>(request.Activity);
                appDbContext.Activities.Add(activity);
                var attendee = new ActivityAttendee
                {
                    User = user,
                    Activity = activity,
                    IsHost = true
                };
                appDbContext.ActivityAttendees.Add(attendee);
                bool result = await appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                {
                    return Result<string>.Failure("Failed to create activity", 400);
                }
                return Result<string>.Success(activity.Id.ToString());
            }
        }
    }
}
