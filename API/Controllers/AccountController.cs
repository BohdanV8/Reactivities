using API.Entities;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController(SignInManager<User> signInManager) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterEntity registerEntity)
        {
            User user = new User
            {
                UserName = registerEntity.Email,
                Email = registerEntity.Email,
                DisplayName = registerEntity.DisplayName
            };

            IdentityResult result = await signInManager.UserManager.CreateAsync(user, registerEntity.Password);
            if (result.Succeeded) return Ok();

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem();
        }
        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated is false)
                return NoContent();

            User user = await signInManager.UserManager.GetUserAsync(User);

            if (user is null)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                user.DisplayName,
                user.Email,
                user.Id,
                user.ImageUrl
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return NoContent();
        }
    }
}
