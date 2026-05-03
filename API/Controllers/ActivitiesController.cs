using System;
using Microsoft.AspNetCore.Mvc;
using Domain;
using MediatR;
using Application.Activities.Queries;
using Application.Activities.Commands;
namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ActivitiesController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<List<Activity>>> GetActivities()
    {
        return await Mediator.Send(new GetActivityList.Query());
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Activity>> GetActivity(Guid id)
    {
        return await Mediator.Send(new GetActivityDetails.Query(id));
    }

    [HttpPost]
    public async Task<ActionResult<string>> CreateActivity([FromBody] Activity activity)
    {
        var command = new CreateActivity.Command { Activity = activity };
        string result = await Mediator.Send(command);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> EditActivity([FromBody] Activity activity)
    {
        var command = new EditActivity.Command(activity);
        await Mediator.Send(command);
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(Guid id)
    {
        var command = new DeleteActivity.Command(id);
        await Mediator.Send(command);
        return Ok("Activity deleted");
    }

}