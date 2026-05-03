using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Queries
{
    public class GetActivityDetails
    {
        public class Query : IRequest<Activity>
        {
            public Query(Guid id) { this.id = id; }
            public Guid id { get; set; }
        }

        public class Handler (AppDbContext appDbContext) : IRequestHandler<Query, Activity>
        {
            public async Task<Activity> Handle(Query request, CancellationToken cancellationToken)
            {
                Activity? activity = await appDbContext.Activities.FindAsync(request.id, cancellationToken);
                if (activity == null)
                {
                    throw new Exception("Activity not found");
                }
                else
                {
                    return activity;
                }
            }
        }
    }
}
