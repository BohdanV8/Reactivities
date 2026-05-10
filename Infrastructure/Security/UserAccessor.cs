using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Application.Intarfaces;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Security
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _appDbContext;

        public UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext appDbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
        }

        public async Task<User> GetUserAsync()
        {
            return await _appDbContext.Users.FindAsync(getUserId())
                ?? throw new UnauthorizedAccessException("No user is logged in");

        }

        public string getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new Exception("User not found");
        }

        public async Task<User> GetUserWithPhotosAsync()
        {
            string userId = getUserId();
            return await _appDbContext.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.Id == userId)
                ?? throw new UnauthorizedAccessException("No user is logged in");
        }
    }
}
