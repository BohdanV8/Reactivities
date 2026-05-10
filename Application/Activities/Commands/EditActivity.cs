using Domain;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Application.Core;
using Application.Activities.DTOs;

namespace Application.Activities.Commands
{
    public class EditActivity
    {
        public class Command : IRequest<Result<Unit>>
        {
            public EditActivityEntity activity { get; set; }
            public Command(EditActivityEntity activity)
            {
                this.activity = activity;
            }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly AppDbContext appDbContext;
            private readonly IMapper mapper;
            public Handler(AppDbContext appDbContext, IMapper mapper)
            {
                this.appDbContext = appDbContext;
                this.mapper = mapper;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                Activity activity = await appDbContext.Activities.FindAsync([Guid.TryParse(request.activity.Id, out var guid) ? guid : throw new Exception("Invalid ID")], cancellationToken) ?? throw new Exception("Cannot find activity");
                if (activity is null)
                {
                    return Result<Unit>.Failure("Cannot find activity", 404);
                }
                mapper.Map(request.activity, activity);
                var result = await appDbContext.SaveChangesAsync(cancellationToken) > 0;
                if (!result)
                {
                    return Result<Unit>.Failure("Failed to update the activity", 400);
                }
                return Result<Unit>.Success(Unit.Value);

            }
        }
    }
}
