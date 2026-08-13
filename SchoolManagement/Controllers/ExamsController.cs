using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ExamDTOs;
using School.DTO.ExamDTOs.Requests;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamResponse>>> GetAllExams()
        {
            return Ok(await _examService.GetAllExamsAsync());
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamResponse>> GetExamById(int id)
        {
            ExamResponse exam = await _examService.GetExamByIdAsync(id);

            return Ok(exam);
        }

        [HttpGet("Class/{classId:int}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamResponse>>> GetExamsByClassId(int classId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsByClassIdAsync(classId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified class.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamResponse>>> GetExamsByTeacherId(int teacherId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsByTeacherIdAsync(teacherId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified teacher.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpGet("Subject/{subjectId:int}")]
        [Authorize(Policy = "Exams.View")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamResponse>>> GetExamsBySubjectId(int subjectId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsBySubjectIdAsync(subjectId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified subject.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpPost]
        [Authorize(Policy = "Exams.Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddExam(CreateExamRequest exam)
        {
            int examId = await _examService.AddExamAsync(exam);

            return CreatedAtAction(
                nameof(GetExamById),
                new { id = examId },
                examId);
        }

        [HttpPut("{examId:int}")]
        [Authorize(Policy = "Exams.Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateExam(int examId, UpdateExamRequest exam)
        {
            await _examService.UpdateExamAsync(examId, exam);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "Exams.Delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteExam(int id)
        {
            await _examService.DeleteExamAsync(id);

            return NoContent();
        }
    }
}