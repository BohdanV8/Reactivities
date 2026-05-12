using Application.Activities.Commands;
using Application.Activities.Entities;
using Application.Activities.Queries;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class CommentHub : Hub
    {
        private readonly IMediator _mediator;
        public CommentHub(IMediator mediator) { 
            _mediator = mediator; 
        }
        public async Task SendComment(AddComment.Command createCommentEntity)
        {
            Result<CommentEntity> result = await _mediator.Send(createCommentEntity);
            if (result.isSuccess)
            {
                await Clients.Group(createCommentEntity.ActivityId.ToString()).SendAsync("ReceiveComment", result.Value);
            }
        }
        public override async Task OnConnectedAsync()
        {
            HttpContext? httpContext = Context.GetHttpContext();
            var activityId = httpContext?.Request.Query["activityId"];
            if (string.IsNullOrEmpty(activityId))
            {
                throw new HubException("no activity id provided.");
            }
            await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);
            Result<List<CommentEntity>> result = await _mediator.Send(new GetComments.Query { ActivityId = activityId! });
            await Clients.Caller.SendAsync("LoadComments", result.Value);
        }
    }
}
