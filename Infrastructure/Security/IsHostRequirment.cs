using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Security
{
    public class IsHostRequirment : IAuthorizationRequirement
    {
    }

    public class IsHostRequirementHandler : AuthorizationHandler<IsHostRequirment>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsHostRequirementHandler(AppDbContext appDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirment requirment)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return;
            }

            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.GetRouteValue("id") is not string activityId)
            {
                return;
            }
            if (!Guid.TryParse(activityId, out Guid parsedActivityId)) return;
            ActivityAttendee? attendee = await _appDbContext.ActivityAttendees.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == parsedActivityId);
            if (attendee == null)
            {
                return;
            }
            if (attendee.IsHost)
            {
                context.Succeed(requirment);
            }
        }
    }
}
