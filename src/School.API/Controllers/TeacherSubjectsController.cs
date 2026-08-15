using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Requests;
using School.DTO.AssociationsDTOs.TeacherSubjectDTOs.Responses;

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
        [Authorize(Policy = "Teachers.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<TeacherSubjectResponse>>> GetAllTeacherSubjects()
        {
            return Ok(await _teacherSubjectService.GetAllTeacherSubjectsAsync());
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [Authorize(Policy = "Teachers.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TeacherSubjectResponse>>> GetSubjectsByTeacherId(int teacherId)
        {
            List<TeacherSubjectResponse> subjects =
                await _teacherSubjectService.GetSubjectsByTeacherIdAsync(teacherId);

            return Ok(subjects);
        }

        [HttpGet("Subject/{subjectId:int}")]
        [Authorize(Policy = "Teachers.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TeacherSubjectResponse>>> GetTeachersBySubjectId(byte subjectId)
        {
            List<TeacherSubjectResponse> teachers =
                await _teacherSubjectService.GetTeachersBySubjectIdAsync(subjectId);

            return Ok(teachers);
        }

        [HttpPost]
        [Authorize(Policy = "Teachers.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignSubjectToTeacher(
            TeacherSubjectRequest relation)
        {
            await _teacherSubjectService.AssignSubjectToTeacherAsync(relation);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Policy = "Teachers.Update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveSubjectFromTeacher(
            TeacherSubjectRequest relation)
        {
            await _teacherSubjectService.RemoveSubjectFromTeacherAsync(relation);

            return NoContent();
        }
    }
}