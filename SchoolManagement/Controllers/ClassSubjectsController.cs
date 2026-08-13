using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.ClassSubjectDTOs.Responses;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassSubjectsController : ControllerBase
    {
        private readonly IClassSubjectService _classSubjectService;

        public ClassSubjectsController(IClassSubjectService classSubjectService)
        {
            _classSubjectService = classSubjectService;
        }

        [HttpGet]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectResponse>>> GetAllClassSubjects()
        {
            return Ok(await _classSubjectService.GetAllClassSubjectsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClassSubjectResponse>> GetClassSubjectById(int id)
        {
            ClassSubjectResponse classSubject =
                await _classSubjectService.GetClassSubjectByIdAsync(id);

            return Ok(classSubject);
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectResponse>>> GetClassSubjectsByClassId(int classId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsByClassIdAsync(classId));
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectResponse>>> GetClassSubjectsByTeacherId(int teacherId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsByTeacherIdAsync(teacherId));
        }

        [HttpGet("Subject/{subjectId:int}")]
        [Authorize(Policy = "Classes.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ClassSubjectResponse>>> GetClassSubjectsBySubjectId(byte subjectId)
        {
            return Ok(await _classSubjectService.GetClassSubjectsBySubjectIdAsync(subjectId));
        }

        [HttpPost]
        [Authorize(Policy = "Classes.Update")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddClassSubject(CreateClassSubjectRequest classSubject)
        {
            int classSubjectId =
                await _classSubjectService.AddClassSubjectAsync(classSubject);

            return CreatedAtAction(
                nameof(GetClassSubjectById),
                new { id = classSubjectId },
                classSubjectId);
        }

        [HttpPut("{classsubjectID:int}")]
        [Authorize(Policy = "Classes.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateClassSubject(int classsubjectID, UpdateClassSubjectRequest classSubject)
        {
            await _classSubjectService.UpdateClassSubjectAsync(classsubjectID, classSubject);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Classes.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteClassSubject(int id)
        {
            await _classSubjectService.DeleteClassSubjectAsync(id);

            return NoContent();
        }
    }
}