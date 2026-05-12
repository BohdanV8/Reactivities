using Application.Activities.Entities;
using Application.Core;
using Application.Intarfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class AddComment
    {
        public class Command : IRequest<Result<CommentEntity>>
        {
            public required string Body { get; set; }
            public required string ActivityId { get; set; }
        }

        public class Handler(AppDbContext appDbContext, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<CommentEntity>>
        {
            public async Task<Result<CommentEntity>> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await appDbContext.Activities
                    .Include(a => a.Comments)
                    .ThenInclude(x => x.User)
                    .FirstOrDefaultAsync(a => a.Id == Guid.Parse(request.ActivityId), cancellationToken);
                if (activity == null) return Result<CommentEntity>.Failure("Activity not found", 404);
                User user = await userAccessor.GetUserAsync();
                Comment comment = new Comment
                {
                    Body = request.Body,
                    Activity = activity,
                    UserId = user.Id,
                    ActivityId = activity.Id.ToString(),  
                };

                activity.Comments.Add(comment);
                bool result = await appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<CommentEntity>.Failure("Failed to add comment", 400);
                return Result<CommentEntity>.Success(mapper.Map<CommentEntity>(comment));
            }
        }
    }
}
