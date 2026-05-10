using Application.Core;
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
        public class Command : IRequest<Result<Unit>>
        {
            public Guid ID { get; private set; }
            public Command(Guid id)
            {
                this.ID = id;
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext appDbContext;
            public Handler(AppDbContext appDbContext)
            {
                this.appDbContext = appDbContext;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity? activity = await appDbContext.Activities.FindAsync(request.ID, cancellationToken) ??
                    throw new Exception("Cannot find activity");


                if(activity is null)
                {
                    return Result<Unit>.Failure("Cannot find activity", 404);
                }
                appDbContext.Remove(activity);
                var result = await appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                {
                    return Result<Unit>.Failure("Failed to delete activity", 400);
                }
                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
