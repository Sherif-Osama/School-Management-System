using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.UserDTOs.Requests;
using School.DTO.UserDTOs.Responses;
using System.Security.Claims;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUserService userService, IAuthorizationService authorizationService,
            ILogger<UsersController> logger)
        {
            _userService = userService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Users.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserResponse>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Users.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            UserResponse user = await _userService.GetUserByIdAsync(id);

            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(user.PersonID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(user);
        }

        [HttpGet("Search")]
        [Authorize(Policy = "Users.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> GetUserByUsername([FromQuery] string username)
        {
            UserResponse user = await _userService.GetUserByUsernameAsync(username);

            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(user.PersonID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(user);
        }

        [HttpGet("Person/{personId:int}")]
        [Authorize(Policy = "Users.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserResponse>> GetUserByPersonId(int personId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(personId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            UserResponse user = await _userService.GetUserByPersonIdAsync(personId);

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Policy = "Users.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddUser(CreateUserRequest user)
        {
            int userId =
                await _userService.AddUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = userId }, userId);
        }

        [HttpPut("{userId:int}")]
        [Authorize(Policy = "Users.Update.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UpdateUserRequest user)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.Update.All"))
            {
                UserResponse targetUser = await _userService.GetUserByIdAsync(userId);

                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(targetUser.PersonID), new OwnershipRequirement());
                if (!authResult.Succeeded)
                    return Forbid();
            }

            await _userService.UpdateUserAsync(userId, user);

            return Ok();
        }

        [HttpPut("ChangePassword/{userId:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword(int userId, [FromBody] UpdatePasswordRequest changePassword)
        {

            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.Update.All"))
            {
                UserResponse targetUser = await _userService.GetUserByIdAsync(userId);

                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(targetUser.PersonID), new OwnershipRequirement());
                if (!authResult.Succeeded)
                    return Forbid();
            }

            await _userService.ChangePasswordAsync(userId, changePassword);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Users.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            _logger.LogWarning("User {UserId} was deleted by {Username}.", id, User.FindFirstValue(ClaimTypes.Name));
            return NoContent();
        }
    }
}