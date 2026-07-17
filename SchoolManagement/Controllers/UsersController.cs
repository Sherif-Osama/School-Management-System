using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.UserDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDetailsDTO>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetailsDTO>> GetUserById(int id)
        {
            UserDetailsDTO? user =
                await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("Search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetailsDTO>> GetUserByUsername(
            [FromQuery] string username)
        {
            UserDetailsDTO? user =
                await _userService.GetUserByUsernameAsync(username);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("Person/{personId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetailsDTO>> GetUserByPersonId(int personId)
        {

            UserDetailsDTO? user = await _userService.GetUserByPersonIdAsync(personId);
            if (user is null)
                return NotFound("User not found.");

            return Ok(user);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddUser(UserDTO user)
        {
            int userId =
                await _userService.AddUserAsync(user);

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = userId },
                userId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(UpdateUserDTO user)
        {
            await _userService.UpdateUserAsync(user);

            return Ok();
        }

        [HttpPut("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(
            UpdatePasswordDTO changePassword)
        {
            await _userService.ChangePasswordAsync(changePassword);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);

            return NoContent();
        }
    }
}