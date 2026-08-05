using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.PermissionDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PermissionDTO>>> GetAllPermissions()
        {
            return Ok(await _permissionService.GetAllPermissionsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionDTO>> GetPermissionById(int id)
        {
            PermissionDTO? permission = await _permissionService.GetPermissionByIdAsync(id);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        [HttpGet("Name/{permissionName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PermissionDTO>> GetPermissionByName(string permissionName)
        {
            PermissionDTO? permission = await _permissionService.GetPermissionByNameAsync(permissionName);

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddPermission(PermissionDTO permission)
        {
            int permissionId = await _permissionService.AddPermissionAsync(permission);

            return CreatedAtAction(
                nameof(GetPermissionById),
                new { id = permissionId },
                permissionId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePermission(PermissionDTO permission)
        {
            await _permissionService.UpdatePermissionAsync(permission);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePermission(int id)
        {
            await _permissionService.DeletePermissionAsync(id);

            return NoContent();
        }
    }
}
