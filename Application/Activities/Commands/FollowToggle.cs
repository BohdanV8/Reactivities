using Application.Core;
using Application.Intarfaces;
using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string TargetUserId { get; set; } = string.Empty;
        }
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext _appDbContext;
            private readonly IUserAccessor _userAccessor;
            public Handler(AppDbContext appDbContext, IUserAccessor userAccessor)
            {
                _appDbContext = appDbContext;
                _userAccessor = userAccessor;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                User observer = await _userAccessor.GetUserAsync();
                User? target = await _appDbContext.Users.FindAsync([request.TargetUserId], cancellationToken);

                if(target == null) return Result<Unit>.Failure("User not found", 404);

                var following = await _appDbContext.UserFollowings.FindAsync([observer.Id, target.Id], cancellationToken);
                if (following == null)
                {
                    following = new UserFollowing
                    {
                        ObserverId = observer.Id,
                        TargetId = target.Id
                    };
                    _appDbContext.UserFollowings.Add(following);

                }
                else
                {
                    _appDbContext.UserFollowings.Remove(following);
                }
                return await _appDbContext.SaveChangesAsync(cancellationToken) > 0
                    ? Result<Unit>.Success(Unit.Value)
                    : Result<Unit>.Failure("Problem updating following", 500);
            }
        }
    }
}
