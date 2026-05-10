using System;
using Microsoft.AspNetCore.Mvc;
using Domain;
using MediatR;
using Application.Activities.Queries;
using Application.Activities.Commands;
using Application.Activities.DTOs;
using Application.Core;
using Microsoft.AspNetCore.Authorization;
using Application.Activities.Entities;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivitiesController : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<Result<List<ActivityEntity>>>> GetActivities(CancellationToken cancellationToken)
    {
        return HandleResult(await Mediator.Send(new GetActivityList.Query(), cancellationToken));
    }
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Result<ActivityEntity>>> GetActivity(Guid id)
    {
        return HandleResult(await Mediator.Send(new GetActivityDetails.Query(id)));
    }

    [HttpPost]
    public async Task<ActionResult<Result<string>>> CreateActivity([FromBody] CreateActivityEntity activity)
    {
        var command = new CreateActivity.Command { Activity = activity };
        Result<string> result = await Mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("{id}/attend")]
    public async Task<IActionResult> UpdateAttendance(Guid id)
    {
        UpdateAttendance.Command command = new UpdateAttendance.Command { Id = id.ToString() };
        return HandleResult(await Mediator.Send(command));
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "IsActivityHost")]
    public async Task<IActionResult> EditActivity(string id,[FromBody] EditActivityEntity activity)
    {
        activity.Id = id;
        var command = new EditActivity.Command(activity);
        return HandleResult(await Mediator.Send(command));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        var command = new DeleteActivity.Command(id);
        return HandleResult(await Mediator.Send(command));
    }

}