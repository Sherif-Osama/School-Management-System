using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ExamTypeDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamTypesController : ControllerBase
    {
        private readonly IExamTypeService _examTypeService;

        public ExamTypesController(IExamTypeService examTypeService)
        {
            _examTypeService = examTypeService;
        }

        [HttpGet]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamTypeDTO>>> GetAllExamTypes()
        {
            return Ok(await _examTypeService.GetAllExamTypesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamTypeDTO>> GetExamTypeById(int id)
        {
            ExamTypeDTO? examType = await _examTypeService.GetExamTypeByIdAsync(id);

            if (examType == null)
                return NotFound();

            return Ok(examType);
        }

        [HttpGet("Name/{examName}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamTypeDTO>> GetExamTypeByName(string examName)
        {
            ExamTypeDTO? examType = await _examTypeService.GetExamTypeByNameAsync(examName);

            if (examType == null)
                return NotFound();

            return Ok(examType);
        }

        [HttpPost]
        [Authorize(Policy = "Exams.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddExamType(ExamTypeDTO examType)
        {
            int examTypeId = await _examTypeService.AddExamTypeAsync(examType);

            return CreatedAtAction(
                nameof(GetExamTypeById),
                new { id = examTypeId },
                examTypeId);
        }

        [HttpPut]
        [Authorize(Policy = "Exams.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateExamType(ExamTypeDTO examType)
        {
            await _examTypeService.UpdateExamTypeAsync(examType);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Exams.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteExamType(int id)
        {
            await _examTypeService.DeleteExamTypeAsync(id);

            return NoContent();
        }
    }
}