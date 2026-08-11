using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.RoleDTOs.Requests;
using School.DTO.RoleDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RoleResponse>>> GetAllRoles()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponse>> GetRoleById(int id)
        {
            RoleResponse? role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpGet("Name/{roleName}")]
        [Authorize(Policy = "Roles.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleResponse>> GetRoleByName(string roleName)
        {
            RoleResponse? role = await _roleService.GetRoleByNameAsync(roleName);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        [Authorize(Policy = "Roles.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddRole(CreateRoleRequest role)
        {
            int roleId = await _roleService.AddRoleAsync(role);

            return CreatedAtAction(
                nameof(GetRoleById), new { id = roleId }, roleId);
        }

        [HttpPut("{roleId:int}")]
        [Authorize(Policy = "Roles.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRole(int roleId, UpdateRoleRequest role)
        {
            await _roleService.UpdateRoleAsync(roleId, role);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Roles.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);

            return NoContent();
        }
    }
}