using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.Data;
using System.Linq;
using System.Threading.Tasks;

namespace StudentRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentSubjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentSubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/studentsubjects/student/5
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetSubjectsForStudent(int studentId)
        {
            var subjects = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .Include(ss => ss.Subject)
                    .ThenInclude(s => s.Professor) // Incluye al profesor relacionado
                .Select(ss => new
                {
                    Id = ss.Subject.Id,
                    Name = ss.Subject.Name,
                    ProfessorName = ss.Subject.Professor != null ? ss.Subject.Professor.Name : "Sin profesor"
                })
                .ToListAsync();

            if (subjects == null || subjects.Count == 0)
            {
                return NotFound("No se encontraron materias para este estudiante.");
            }

            return Ok(subjects);
        }
    }
}
