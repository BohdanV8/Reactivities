using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Application.Activities.Queries
{
    public class GetActivityList
    {
        public class Query() : IRequest<List<Activity>>
        {
        }

        public class Handler(AppDbContext appDbContext, ILogger<GetActivityList> logger) : IRequestHandler<Query, List<Activity>>
        {
            public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await Task.Delay(100, cancellationToken);
                        logger.LogInformation($"task {i} has completed");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (you can use a logging framework like Serilog, NLog, etc.)
                    Console.WriteLine($"An error occurred while fetching activities: {ex.Message}");
                    // Optionally, you can rethrow the exception or return an empty list
                    throw; // Rethrow the exception to be handled by a global exception handler
                }
                return await appDbContext.Activities.ToListAsync(cancellationToken);
            }
        }
    }
}
