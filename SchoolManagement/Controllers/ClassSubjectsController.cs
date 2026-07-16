using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSubjectsController : ControllerBase
    {
        private readonly ClassSubjectService _classSubjectService;

        public ClassSubjectsController(ClassSubjectService classSubjectService)
        {
            _classSubjectService = classSubjectService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectDetailsDTO>>> GetAllClassSubjects()
        {
            return Ok(await _classSubjectService.GetAllClassSubjectsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassSubjectDetailsDTO>> GetClassSubjectById(int id)
        {
            ClassSubjectDetailsDTO? classSubject =
                await _classSubjectService.GetClassSubjectByIdAsync(id);

            if (classSubject == null)
                return NotFound();

            return Ok(classSubject);
        }

        [HttpGet("Class/{classId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectDetailsDTO>>> GetClassSubjectsByClassId(int classId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsByClassIdAsync(classId));
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectDetailsDTO>>> GetClassSubjectsByTeacherId(int teacherId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsByTeacherIdAsync(teacherId));
        }

        [HttpGet("Subject/{subjectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectDetailsDTO>>> GetClassSubjectsBySubjectId(byte subjectId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsBySubjectIdAsync(subjectId));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClassSubject(ClassSubjectDTO classSubject)
        {
            int classSubjectId =
                await _classSubjectService.AddClassSubjectAsync(classSubject);

            return CreatedAtAction(
                nameof(GetClassSubjectById),
                new { id = classSubjectId },
                classSubjectId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassSubject(ClassSubjectDTO classSubject)
        {
            await _classSubjectService.UpdateClassSubjectAsync(classSubject);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClassSubject(int id)
        {
            await _classSubjectService.DeleteClassSubjectAsync(id);

            return NoContent();
        }
    }
}