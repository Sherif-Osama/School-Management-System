using Microsoft.AspNetCore.Mvc;
using School.BLL;
using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentGradesController : ControllerBase
    {
        private readonly StudentGradeService _studentGradeService;

        public StudentGradesController(StudentGradeService studentGradeService)
        {
            _studentGradeService = studentGradeService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetAllStudentGrades()
        {
            return Ok(await _studentGradeService.GetAllStudentGradesAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentGradeDetailsDTO>> GetStudentGradeById(int id)
        {
            StudentGradeDetailsDTO? studentGrade = await _studentGradeService.GetStudentGradeByIdAsync(id);

            if (studentGrade == null)
                return NotFound();

            return Ok(studentGrade);
        }

        [HttpGet("Student/{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByStudentId(int studentId)
        {
            return Ok(await _studentGradeService.GetStudentGradesByStudentIdAsync(studentId));
        }

        [HttpGet("Exam/{examId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByExamId(int examId)
        {
            return Ok(await _studentGradeService.GetStudentGradesByExamIdAsync(examId));
        }

        [HttpGet("Class/{classId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByClassId(int classId)
        {
            return Ok(await _studentGradeService.GetStudentGradesByClassIdAsync(classId));
        }

        [HttpGet("Subject/{subjectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesBySubjectId(int subjectId)
        {
            return Ok(await _studentGradeService.GetStudentGradesBySubjectIdAsync(subjectId));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddStudentGrade(StudentGradeDTO studentGradeDTO)
        {
            int studentGradeId = await _studentGradeService.AddStudentGradeAsync(studentGradeDTO);

            return CreatedAtAction(
                nameof(GetStudentGradeById),
                new { id = studentGradeId },
                studentGradeId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudentGrade(StudentGradeDTO studentGradeDTO)
        {
            await _studentGradeService.UpdateStudentGradeAsync(studentGradeDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudentGrade(int id)
        {
            await _studentGradeService.DeleteStudentGradeAsync(id);

            return NoContent();
        }
    }
}