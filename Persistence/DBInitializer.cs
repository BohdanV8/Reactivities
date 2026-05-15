using System;
using System.Collections.Generic;
using System.Text;
using Bogus;
using Domain;
using Microsoft.AspNetCore.Identity;
namespace Persistence
{
    public static class DBInitializer
    {
        public static async Task seedData(AppDbContext context, UserManager<User> _userManager)
        {
            List<User> users = new List<User>() {
                new() { Id = "jack-id", DisplayName = "Jack", UserName = "jack@gmail.com", Email = "jack@gmail.com" },
                new() { Id = "tom-id", DisplayName = "Tom", UserName = "tom@gmail.com", Email = "tom@gmail.com" },
                new() { Id = "jane-id", DisplayName = "Jane", UserName = "jane@gmail.com", Email = "jane@gmail.com" },
            };
            if (!_userManager.Users.Any())
            {
                foreach (var user in users)
                {
                    await _userManager.CreateAsync(user, "Pa$$w0rd");
                }
            }

            if (context.Activities.Any()) return;
            string[] categories = new[] { "drinks", "culture", "music", "travel", "film" };

            Faker<Activity> activityFaker = new Faker<Activity>()
                .RuleFor(a => a.Title, f => f.Lorem.Sentence(3).TrimEnd('.'))
                .RuleFor(a => a.Date, f => f.Date.Between(DateTime.Now.AddMonths(-2), DateTime.Now.AddMonths(10)))
                .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
                .RuleFor(a => a.Category, f => f.PickRandom(categories))
                .RuleFor(a => a.City, f => f.Address.City())
                .RuleFor(a => a.Venue, f => f.Company.CompanyName())
                .RuleFor(a => a.Latitude, f => f.Address.Latitude())
                .RuleFor(a => a.Longitude, f => f.Address.Longitude());

            List<Activity> activities = activityFaker.Generate(1000);

            Random random = new Random();

            foreach (Activity activity in activities)
            {
                int attendeesCount = random.Next(1, 4);

                // Перемішуємо наших 3 юзерів і беремо потрібну кількість
                List<User> randomUsers = users.OrderBy(x => random.Next()).Take(attendeesCount).ToList();

                activity.Attendees = new List<ActivityAttendee>();

                for (int i = 0; i < randomUsers.Count; i++)
                {
                    activity.Attendees.Add(new ActivityAttendee
                    {
                        UserId = randomUsers[i].Id,
                        // Перший юзер у випадковому списку завжди буде Хостом
                        IsHost = (i == 0)
                    });
                }
            }
            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();
        }
    }
}
