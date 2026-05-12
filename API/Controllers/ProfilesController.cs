using Application.Profiles.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Application.Profiles.Profiles;
using Application.Profiles.Entities;
using Application.Profiles.Queries;
using Application.Activities.Commands;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfilesController : BaseApiController
    {
        [HttpPost("add-photo")]
        public async Task<ActionResult> AddNewPhoto([FromForm] IFormFile formFile)
        {
            return HandleResult(await Mediator.Send(new AddPhoto.Command { File = formFile }));
        }

        [HttpGet("{userId}/photos")]
        public async Task<ActionResult> GetPhotosForUser(string userId)
        {
            return HandleResult(await Mediator.Send(new GetProfilePhotos.Query { UserId = userId }));
        }

        [HttpDelete("{photoId}/photos")]
        public async Task<ActionResult> DeletePhoto(string photoId)
        {
            return HandleResult(await Mediator.Send(new DeletePhoto.Command { PhotoId = photoId }));
        }

        [HttpPut("{photoId}/setMain")]
        public async Task<ActionResult> SetMainPhoto(string photoId)
        {
            return HandleResult(await Mediator.Send(new SetMainPhoto.Command { PhotoId = photoId }));
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(string userId)
        {
            return HandleResult(await Mediator.Send(new GetProfile.Query { UserId = userId }));
        }

        [HttpPut("editProfile")]
        public async Task<ActionResult> EditUserProfile([FromForm] EditProfileEntity profileEntity)
        {
            EditProfile.Command command = new EditProfile.Command(profileEntity);
            return HandleResult(await Mediator.Send(command));
        }

        [HttpPost("{userId}/follow")]
        public async Task<ActionResult> FollowToggle(string userId)
        {
            return HandleResult(await Mediator.Send(new FollowToggle.Command { TargetUserId = userId }));
        }

        [HttpPost("{userId}/followings")]
        public async Task<ActionResult> GetFollowings(string userId, [FromQuery] string predicate = "followers")
        {
            return HandleResult(await Mediator.Send(new GetFollowings.Query { UserId = userId, Predicate = predicate }));
        }

        [HttpGet("{userId}/follow-list")]
        public async Task<ActionResult> GetFollowList(string userId, string predicate)
        {
            return HandleResult(await Mediator.Send(new GetFollowings.Query { UserId = userId, Predicate = predicate }));
        }
    }
}
