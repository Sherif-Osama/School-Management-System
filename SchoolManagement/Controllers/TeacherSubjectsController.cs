using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherSubjectsController : ControllerBase
    {
        private readonly ITeacherSubjectService _teacherSubjectService;

        public TeacherSubjectsController(ITeacherSubjectService teacherSubjectService)
        {
            _teacherSubjectService = teacherSubjectService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeacherSubjectDetailsDTO>>> GetAllTeacherSubjects()
        {
            return Ok(await _teacherSubjectService.GetAllTeacherSubjectsAsync());
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TeacherSubjectDetailsDTO>>> GetSubjectsByTeacherId(int teacherId)
        {
            List<TeacherSubjectDetailsDTO> subjects =
                await _teacherSubjectService.GetSubjectsByTeacherIdAsync(teacherId);

            return Ok(subjects);
        }

        [HttpGet("Subject/{subjectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TeacherSubjectDetailsDTO>>> GetTeachersBySubjectId(byte subjectId)
        {
            List<TeacherSubjectDetailsDTO> teachers =
                await _teacherSubjectService.GetTeachersBySubjectIdAsync(subjectId);

            return Ok(teachers);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignSubjectToTeacher(
            TeacherSubjectDTO relation)
        {
            await _teacherSubjectService.AssignSubjectToTeacherAsync(relation);

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveSubjectFromTeacher(
            TeacherSubjectDTO relation)
        {
            await _teacherSubjectService.RemoveSubjectFromTeacherAsync(relation);

            return NoContent();
        }
    }
}