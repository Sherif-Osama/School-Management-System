using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.UserDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public UsersController(IUserService userService, IAuthorizationService authorizationService)
        {
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Users.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDetailsDTO>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Users.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<UserDetailsDTO>> GetUserById(int id)
        {
            UserDetailsDTO? user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

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
        public async Task<ActionResult<UserDetailsDTO>> GetUserByUsername([FromQuery] string username)
        {
            UserDetailsDTO? user = await _userService.GetUserByUsernameAsync(username);

            if (user == null)
                return NotFound();

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
        public async Task<ActionResult<UserDetailsDTO>> GetUserByPersonId(int personId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(personId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            UserDetailsDTO? user = await _userService.GetUserByPersonIdAsync(personId);
            if (user is null)
                return NotFound("User not found.");

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Policy = "Users.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddUser(UserDTO user)
        {
            int userId =
                await _userService.AddUserAsync(user);

            return CreatedAtAction(nameof(GetUserById), new { id = userId }, userId);
        }

        [HttpPut]
        [Authorize(Policy = "Users.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO user)
        {
            await _userService.UpdateUserAsync(user);

            return Ok();
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword(UpdatePasswordDTO changePassword)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Users.Update"))
            {
                UserDetailsDTO? targetUser = await _userService.GetUserByIdAsync(changePassword.UserID);

                if (targetUser is null)
                    return NotFound();

                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new PersonOwnedResource(targetUser.PersonID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            await _userService.ChangePasswordAsync(changePassword);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Users.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);

            return NoContent();
        }
    }
}