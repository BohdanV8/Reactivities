using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Domain;
namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivitiesController (AppDbContext appDbContext) : ControllerBase
{
    
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> GetActivities()
        {
            var activities = await appDbContext.Activities.ToListAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(Guid id)
        {
            var activity = await appDbContext.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            return Ok(activity);
        }
}
