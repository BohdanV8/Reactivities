using System;
using System.Collections.Generic;
using System.Text;
using Domain;
namespace Persistence
{
    public static class DBInitializer
    {
        public static async Task seedData(AppDbContext context)
        {
            if (context.Activities.Any())
            {
                return;
            }
            var activities = new List<Activity>
            {
                new Activity
                {
                    Title = "Past Activity 1",
                    Date = DateTime.Now.AddMonths(-2),
                    Description = "Activity 2 months ago",
                    Category = "drinks",
                    City = "London",
                    Venue = "Pub"
                },
                new Activity
                {
                    Title = "Past Activity 2",
                    Date = DateTime.Now.AddMonths(-1),
                    Description = "Activity 1 month ago",
                    Category = "culture",
                    City = "Paris",
                    Venue = "Louvre"
                },
                new Activity
                {
                    Title = "Future Activity 1",
                    Date = DateTime.Now.AddMonths(1),
                    Description = "Activity in 1 month",
                    Category = "music",
                    City = "New York",
                    Venue = "Madison Square Garden"
                },
                new Activity
                {
                    Title = "Future Activity 2",
                    Date = DateTime.Now.AddMonths(2),
                    Description = "Activity in 2 months",
                    Category = "food",
                    City = "Rome",
                    Venue = "Pizzeria"
                }
            };

            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();
        }
    }
}
