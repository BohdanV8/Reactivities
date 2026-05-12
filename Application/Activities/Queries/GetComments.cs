using Application.Activities.Entities;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Activities.Queries
{
    public class GetComments
    {
        public class Query : IRequest<Result<List<CommentEntity>>>
        {
            public required string ActivityId { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<List<CommentEntity>>>
        {
            private readonly AppDbContext _appDbContext;
            private readonly IMapper _mapper;
            public Handler(AppDbContext appDbContext, IMapper mapper)
            {
                _appDbContext = appDbContext;
                _mapper = mapper;
            }
            public async Task<Result<List<CommentEntity>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (!Guid.TryParse(request.ActivityId, out Guid activityId))
                {
                    return Result<List<CommentEntity>>.Failure("Invalid activity id format.", 400);
                }
                List<CommentEntity> comments = await _appDbContext.Comments
                    .Where(x => x.ActivityId == request.ActivityId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ProjectTo<CommentEntity>(_mapper.ConfigurationProvider)
                    .ToListAsync(cancellationToken);
                return Result<List<CommentEntity>>.Success(comments);
            }
        }
    }
}
