using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Requests;
using School.DTO.AssociationsDTOs.UserRoleDTOs.Responses;
using System.Security.Claims;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        private readonly ILogger<UserRolesController> _logger;
        public UserRolesController(IUserRoleService userRoleService, ILogger<UserRolesController> logger)
        {
            _userRoleService = userRoleService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserRoleResponse>>> GetAllUserRoles()
        {
            return Ok(await _userRoleService.GetAllUserRolesAsync());
        }

        [HttpGet("{userId:int}/{roleId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserRoleResponse>> GetUserRole(int userId, int roleId)
        {
            UserRoleResponse userRole =
                await _userRoleService.GetUserRoleAsync(userId, roleId);

            return Ok(userRole);
        }

        [HttpGet("User/{userId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<UserRoleResponse>>> GetRolesByUserId(int userId)
        {
            List<UserRoleResponse> roles =
                await _userRoleService.GetRolesByUserIdAsync(userId);

            if (roles.Count == 0)
                return NotFound("No roles found for the specified user.");

            return Ok(roles);
        }

        [HttpPost]
        [Authorize(Policy = "UserRoles.Assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddUserRole(UserRoleRequest userRole)
        {
            await _userRoleService.AddUserRoleAsync(userRole);

            _logger.LogWarning("Role {RoleId} was assigned to User {UserId} by {Username}.", userRole.RoleID, userRole.UserID, User.FindFirstValue(ClaimTypes.Name));

            return Ok();
        }

        [HttpDelete("{userId:int}/{roleId:int}")]
        [Authorize(Policy = "UserRoles.Assign")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteUserRole(int userId, int roleId)
        {
            await _userRoleService.DeleteUserRoleAsync(userId, roleId);

            _logger.LogWarning("Role {RoleId} was removed from User {UserId} by {Username}.", roleId, userId, User.FindFirstValue(ClaimTypes.Name));

            return NoContent();
        }
    }
}