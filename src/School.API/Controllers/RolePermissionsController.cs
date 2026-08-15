using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;
using System.Security.Claims;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;
        private readonly ILogger<RolePermissionsController> _logger;

        public RolePermissionsController(IRolePermissionService rolePermissionService, ILogger<RolePermissionsController> logger)
        {
            _rolePermissionService = rolePermissionService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RolePermissionResponse>>> GetAllRolePermissions()
        {
            return Ok(await _rolePermissionService.GetAllRolePermissionsAsync());
        }

        [HttpGet("{roleId:int}/{permissionId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RolePermissionResponse>> GetRolePermission(int roleId, int permissionId)
        {
            RolePermissionResponse rolePermission =
                await _rolePermissionService.GetRolePermissionAsync(roleId, permissionId);

            return Ok(rolePermission);
        }

        [HttpGet("Role/{roleId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RolePermissionResponse>>> GetPermissionsByRoleId(int roleId)
        {
            List<RolePermissionResponse> permissions =
                await _rolePermissionService.GetPermissionsByRoleIdAsync(roleId);

            if (permissions.Count == 0)
                return NotFound("No permissions found for the specified role.");

            return Ok(permissions);
        }

        [HttpPost]
        [Authorize(Policy = "UserRoles.Assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRolePermission(RolePermissionRequest rolePermission)
        {
            await _rolePermissionService.AddRolePermissionAsync(rolePermission);

            _logger.LogWarning("Permission {PermissionId} was granted to Role {RoleId} by {Username}.",
            rolePermission.PermissionID, rolePermission.RoleID, User.FindFirstValue(ClaimTypes.Name));

            return Ok();
        }

        [HttpDelete("{roleId:int}/{permissionId:int}")]
        [Authorize(Policy = "Roles.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
        {
            await _rolePermissionService.DeleteRolePermissionAsync(roleId, permissionId);
            _logger.LogWarning("Permission {PermissionId} was revoked from Role {RoleId} by {Username}.",
            permissionId, roleId, User.FindFirstValue(ClaimTypes.Name));
            return NoContent();
        }
    }
}