using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.RolePermissionDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePermissionsController : ControllerBase
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionsController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RolePermissionDetailsDTO>>> GetAllRolePermissions()
        {
            return Ok(await _rolePermissionService.GetAllRolePermissionsAsync());
        }

        [HttpGet("{roleId:int}/{permissionId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RolePermissionDetailsDTO>> GetRolePermission(int roleId, int permissionId)
        {
            RolePermissionDetailsDTO? rolePermission =
                await _rolePermissionService.GetRolePermissionAsync(roleId, permissionId);

            if (rolePermission == null)
                return NotFound();

            return Ok(rolePermission);
        }

        [HttpGet("Role/{roleId:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RolePermissionDetailsDTO>>> GetPermissionsByRoleId(int roleId)
        {
            List<RolePermissionDetailsDTO> permissions =
                await _rolePermissionService.GetPermissionsByRoleIdAsync(roleId);

            if (permissions.Count == 0)
                return NotFound("No permissions found for the specified role.");

            return Ok(permissions);
        }

        [HttpPost]
        [Authorize(Policy = "Roles.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddRolePermission(RolePermissionDTO rolePermission)
        {
            await _rolePermissionService.AddRolePermissionAsync(rolePermission);

            return Ok();
        }

        [HttpDelete("{roleId:int}/{permissionId:int}")]
        [Authorize(Policy = "Roles.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRolePermission(int roleId, int permissionId)
        {
            await _rolePermissionService.DeleteRolePermissionAsync(roleId, permissionId);

            return NoContent();
        }
    }
}