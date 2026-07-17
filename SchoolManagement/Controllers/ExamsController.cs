using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.ExamDTOs;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamDetailsDTO>>> GetAllExams()
        {
            return Ok(await _examService.GetAllExamsAsync());
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExamDetailsDTO>> GetExamById(int id)
        {
            ExamDetailsDTO? exam = await _examService.GetExamByIdAsync(id);

            if (exam == null)
                return NotFound();

            return Ok(exam);
        }

        [HttpGet("Class/{classId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamDetailsDTO>>> GetExamsByClassId(int classId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsByClassIdAsync(classId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified class.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpGet("Teacher/{teacherId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamDetailsDTO>>> GetExamsByTeacherId(int teacherId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsByTeacherIdAsync(teacherId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified teacher.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpGet("Subject/{subjectId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExamDetailsDTO>>> GetExamsBySubjectId(int subjectId)
        {
            var ExamDetailsDTOList = await _examService.GetExamsBySubjectIdAsync(subjectId);

            if (ExamDetailsDTOList.Count <= 0)
                return NotFound("No exams found for the specified subject.");

            return Ok(ExamDetailsDTOList);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<int>> AddExam(ExamDTO exam)
        {
            int examId = await _examService.AddExamAsync(exam);

            return CreatedAtAction(
                nameof(GetExamById),
                new { id = examId },
                examId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateExam(ExamDTO exam)
        {
            await _examService.UpdateExamAsync(exam);

            return Ok();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteExam(int id)
        {
            await _examService.DeleteExamAsync(id);

            return NoContent();
        }
    }
}