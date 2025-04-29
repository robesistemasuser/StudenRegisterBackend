using Microsoft.AspNetCore.Mvc;
using StudentRegistration.API.Data;
using StudentRegistration.API.DTOs;
using StudentRegistration.API.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateDto subjectDto)
        {
            var professor = await _context.Professors.FindAsync(subjectDto.ProfessorId);
            if (professor == null)
                return BadRequest("Invalid Professor Id.");

            var subject = new Subject
            {
                Name = subjectDto.Name,
                Credits = 3,
                ProfessorId = subjectDto.ProfessorId
            };

            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            return Ok(new SubjectDto
            {
                Id = subject.Id,
                Name = subject.Name,
                Credits = subject.Credits,
                ProfessorId = subject.ProfessorId,
                ProfessorName = professor.Name
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAllSubjects()
        {
            var subjects = await _context.Subjects
                .Include(s => s.Professor)
                .Select(s => new SubjectDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Credits = s.Credits,
                    ProfessorId = s.ProfessorId,
                    ProfessorName = s.Professor.Name
                })
                .ToListAsync();

            return Ok(subjects);
        }

        // CORREGIDO: Este método tenía errores con el parámetro mal nombrado
        [HttpPost("{studentId}/subjects")]
        public async Task<IActionResult> AssignSubjects(int studentId, [FromBody] StudentSubjectsSelectionDto selectionDto)
        {
            var student = await _context.Students
                .Include(s => s.StudentSubjects)
                .ThenInclude(ss => ss.Subject)
                .ThenInclude(s => s.Professor)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound();

            if (selectionDto.SubjectIds.Count != 3)
                return BadRequest("You must select exactly 3 subjects.");

            var selectedSubjects = await _context.Subjects
                .Include(s => s.Professor)
                .Where(s => selectionDto.SubjectIds.Contains(s.Id))
                .ToListAsync();

            if (selectedSubjects.Count != 3)
                return BadRequest("Invalid subject IDs.");

            var professorsSelected = selectedSubjects.Select(s => s.ProfessorId).Distinct();
            if (professorsSelected.Count() != 3)
                return BadRequest("Subjects must be taught by different professors.");

            foreach (var subject in selectedSubjects)
            {
                _context.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = studentId,
                    SubjectId = subject.Id
                });
            }

            await _context.SaveChangesAsync();

            return Ok("Subjects assigned successfully.");
        }

        // EXISTENTE: aún útil si quieres mostrar materias con compañeros agrupados
        [HttpGet("/api/students/{studentId}/classmates-by-subject")]
        public async Task<IActionResult> GetClassmatesGroupedBySubjects(int studentId)
        {
            var studentSubjects = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .Select(ss => ss.SubjectId)
                .ToListAsync();

            if (!studentSubjects.Any())
                return BadRequest("Student is not enrolled in any subjects.");

            var classmates = await _context.StudentSubjects
                .Where(ss => studentSubjects.Contains(ss.SubjectId) && ss.StudentId != studentId)
                .Include(ss => ss.Student)
                .Include(ss => ss.Subject)
                .GroupBy(ss => ss.SubjectId)
                .Select(g => new
                {
                    SubjectId = g.Key,
                    SubjectName = g.First().Subject.Name,
                    Classmates = g
                        .Select(x => new
                        {
                            FullName = x.Student.FirstName + " " + x.Student.LastName
                        })
                        .ToList()
                })
                .ToListAsync();

            return Ok(classmates);
        }

        // ✅ NUEVO: Obtener compañeros por materia específica excluyendo al estudiante actual
        [HttpGet("{subjectId}/classmates")]
        public async Task<IActionResult> GetClassmatesBySubject(int subjectId, [FromQuery] int studentId)
        {
            var classmates = await _context.StudentSubjects
                .Where(ss => ss.SubjectId == subjectId && ss.StudentId != studentId)
                .Include(ss => ss.Student)
                .Select(ss => new
                {
                    FullName = ss.Student.FirstName + " " + ss.Student.LastName
                })
                .ToListAsync();

            return Ok(classmates);
        }
    }
}
