using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserRoleDetailsDTO>>> GetAllUserRoles()
        {
            return Ok(await _userRoleService.GetAllUserRolesAsync());
        }

        [HttpGet("{userId:int}/{roleId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserRoleDetailsDTO>> GetUserRole(int userId, int roleId)
        {
            UserRoleDetailsDTO? userRole =
                await _userRoleService.GetUserRoleAsync(userId, roleId);

            if (userRole == null)
                return NotFound();

            return Ok(userRole);
        }

        [HttpGet("User/{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserRoleDetailsDTO>>> GetRolesByUserId(int userId)
        {
            List<UserRoleDetailsDTO> roles =
                await _userRoleService.GetRolesByUserIdAsync(userId);

            if (roles.Count == 0)
                return NotFound("No roles found for the specified user.");

            return Ok(roles);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserRole(UserRoleDTO userRole)
        {
            await _userRoleService.AddUserRoleAsync(userRole);

            return Ok();
        }

        [HttpDelete("{userId:int}/{roleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
        {
            await _userRoleService.DeleteUserRoleAsync(userId, roleId);

            return NoContent();
        }
    }
}
