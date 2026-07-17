using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ParentsDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParentsController : ControllerBase
    {
        private readonly IParentService _parentService;

        public ParentsController(IParentService parentService)
        {
            _parentService = parentService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ParentDetailsDTO>>> GetAllParents()
        {
            return Ok(await _parentService.GetAllParentsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParentDetailsDTO>> GetParentById(int id)
        {
            ParentDetailsDTO? parent = await _parentService.GetParentByIdAsync(id);

            if (parent == null)
                return NotFound();

            return Ok(parent);
        }

        [HttpGet("Person/{personId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParentDetailsDTO>> GetParentByPersonId(int personId)
        {
            ParentDetailsDTO? parent = await _parentService.GetParentByPersonIdAsync(personId);

            if (parent == null)
                return NotFound();

            return Ok(parent);
        }

        [HttpGet("NationalID/{nationalId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ParentDetailsDTO>> GetParentByNationalId(string nationalId)
        {
            ParentDetailsDTO? parent = await _parentService.GetParentByNationalIdAsync(nationalId);

            if (parent == null)
                return NotFound();

            return Ok(parent);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddParent(ParentDTO parentDTO)
        {
            int parentId = await _parentService.AddParentAsync(parentDTO);

            return CreatedAtAction(
                nameof(GetParentById),
                new { id = parentId },
                parentId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateParent(ParentDTO parentDTO)
        {
            await _parentService.UpdateParentAsync(parentDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteParent(int id)
        {
            await _parentService.DeleteParentAsync(id);

            return NoContent();
        }
    }
}