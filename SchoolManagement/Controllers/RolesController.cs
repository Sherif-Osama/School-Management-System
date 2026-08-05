using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.RoleDTOs;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<RoleDTO>>> GetAllRoles()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> GetRoleById(int id)
        {
            RoleDTO? role = await _roleService.GetRoleByIdAsync(id);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpGet("Name/{roleName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDTO>> GetRoleByName(string roleName)
        {
            RoleDTO? role = await _roleService.GetRoleByNameAsync(roleName);

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddRole(RoleDTO role)
        {
            int roleId = await _roleService.AddRoleAsync(role);

            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = roleId },
                roleId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateRole(RoleDTO role)
        {
            await _roleService.UpdateRoleAsync(role);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteRole(int id)
        {
            await _roleService.DeleteRoleAsync(id);

            return NoContent();
        }
    }
}