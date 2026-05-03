using Domain;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Commands
{
    public class EditActivity
    {
        public class Command : IRequest
        {
            public Activity activity { get; set; }
            public Command(Activity activity)
            {
                this.activity = activity;
            }
        }

        public class Handler : IRequestHandler<Command>
        {
            private readonly AppDbContext appDbContext;
            private readonly IMapper mapper;
            public Handler(AppDbContext appDbContext, IMapper mapper)
            {
                this.appDbContext = appDbContext;
                this.mapper = mapper;
            }
            public async Task Handle(Command request, CancellationToken cancellationToken)
            {
                Activity activity = await appDbContext.Activities.FindAsync([request.activity.Id], cancellationToken) ?? throw new Exception("Cannto find activity");
                mapper.Map(request.activity, activity);
                await appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
