using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class DeleteActivity
    {
        public class Command : IRequest
        {
            public Command(Guid id)
            {
                this.id = id;
            }
            public Guid id;

        }

        public class Handler(AppDbContext appDbContext) : IRequestHandler<Command>
        {
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await appDbContext.Activities.FindAsync(request.id, cancellationToken) ?? throw new Exception("Cannot find activity");
                appDbContext.Remove(activity);
                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
