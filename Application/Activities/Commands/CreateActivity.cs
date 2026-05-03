using Domain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<string>
        {
            public required Activity Activity { get; set; }
        }

        public class Handler(AppDbContext appDbContext) : IRequestHandler<Command, string>
        {
            public async Task<string> Handle(Command request, CancellationToken cancellationToken)
            {
                appDbContext.Activities.Add(request.Activity);
                await appDbContext.SaveChangesAsync(cancellationToken);
                return request.Activity.Id.ToString();
            }
        }
    }
}
