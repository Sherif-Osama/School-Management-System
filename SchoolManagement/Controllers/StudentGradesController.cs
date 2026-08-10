using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.API.Authorization.OwnedResources;
using School.API.Authorization.Requirements;
using School.BLL.Authentication;
using School.BLL.Interfaces;
using School.DTO.StudentGradeDetailsDTOs;
using School.DTO.StudentGradeDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentGradesController : ControllerBase
    {
        private readonly IStudentGradeService _studentGradeService;
        private readonly IAuthorizationService _authorizationService;

        public StudentGradesController(IStudentGradeService studentGradeService, IAuthorizationService authorizationService)
        {
            _studentGradeService = studentGradeService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetAllStudentGrades()
        {
            return Ok(await _studentGradeService.GetAllStudentGradesAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Grades.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<StudentGradeDetailsDTO>> GetStudentGradeById(int id)
        {
            StudentGradeDetailsDTO? studentGrade = await _studentGradeService.GetStudentGradeByIdAsync(id);

            if (studentGrade == null)
                return NotFound();

            if (!User.HasClaim(CustomClaimTypes.Permission, "Grades.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(studentGrade.StudentID), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(studentGrade);
        }

        [HttpGet("Student/{studentId:int}")]
        [Authorize(Policy = "Grades.View.Own")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByStudentId(int studentId)
        {
            if (!User.HasClaim(CustomClaimTypes.Permission, "Grades.View.All"))
            {
                var authResult = await _authorizationService.AuthorizeAsync(
                    User, new StudentOwnedResource(studentId), new OwnershipRequirement());

                if (!authResult.Succeeded)
                    return Forbid();
            }

            return Ok(await _studentGradeService.GetStudentGradesByStudentIdAsync(studentId));
        }

        // إرجاع درجات كل الطلاب في امتحان واحد - مفيش معنى لـ "Own" هنا، فضلت .All بس
        [HttpGet("Exam/{examId:int}")]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByExamId(int examId)
        {
            return Ok(await _studentGradeService.GetStudentGradesByExamIdAsync(examId));
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesByClassId(int classId)
        {
            return Ok(await _studentGradeService.GetStudentGradesByClassIdAsync(classId));
        }

        [HttpGet("Subject/{subjectId:int}")]
        [Authorize(Policy = "Grades.View.All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<StudentGradeDetailsDTO>>> GetStudentGradesBySubjectId(int subjectId)
        {
            return Ok(await _studentGradeService.GetStudentGradesBySubjectIdAsync(subjectId));
        }

        [HttpPost]
        [Authorize(Policy = "Grades.Create")]
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
        [Authorize(Policy = "Grades.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> UpdateStudentGrade(StudentGradeDTO studentGradeDTO)
        {
            await _studentGradeService.UpdateStudentGradeAsync(studentGradeDTO);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Grades.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudentGrade(int id)
        {
            await _studentGradeService.DeleteStudentGradeAsync(id);

            return NoContent();
        }
    }
}