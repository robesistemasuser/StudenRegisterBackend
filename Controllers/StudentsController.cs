using Microsoft.AspNetCore.Mvc;
using StudentRegistration.API.Data;
using StudentRegistration.API.DTOs;
using StudentRegistration.API.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/students/register
        [HttpPost("register")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Los datos enviados no son válidos." });

            if (string.IsNullOrWhiteSpace(studentDto.FirstName))
                return BadRequest(new { message = "El nombre es obligatorio." });

            if (studentDto.SubjectIds.Count > 3)
                return BadRequest(new { message = "El estudiante solo puede seleccionar hasta 3 materias." });

            // Validar si ya existe un estudiante con la misma cédula o correo
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Cedula == studentDto.Cedula || s.Email == studentDto.Email);

            if (existingStudent != null)
            {
                return BadRequest(new { message = "Ya existe un estudiante registrado con esta cédula o correo." });
            }

            // Obtener las materias seleccionadas por el estudiante
            var subjects = await _context.Subjects
                .Where(s => studentDto.SubjectIds.Contains(s.Id))
                .Include(s => s.Professor)
                .ToListAsync();

            if (subjects.Count != studentDto.SubjectIds.Count)
                return BadRequest(new { message = "Algunas materias no fueron encontradas." });

            // Validación de créditos
            var totalCredits = subjects.Sum(s => s.Credits);
            if (totalCredits != 9)
            {
                return BadRequest(new { message = "El total de créditos seleccionados debe ser exactamente 9." });
            }

            // Validar que no haya materias del mismo profesor
            var professorIds = subjects.Select(s => s.ProfessorId).Distinct().ToList();
            if (professorIds.Count != subjects.Count)
                return BadRequest(new { message = "No puedes seleccionar dos materias del mismo profesor." });

            // Crear el estudiante
            var student = new Student
            {
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Cedula = studentDto.Cedula,
                Email = studentDto.Email,
                Phone = studentDto.Phone
            };

            // Agregar el estudiante a la base de datos
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Asociar las materias seleccionadas con el estudiante
            foreach (var subject in subjects)
            {
                _context.StudentSubjects.Add(new StudentSubject
                {
                    StudentId = student.Id,
                    SubjectId = subject.Id
                });
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Cedula = student.Cedula,
                Email = student.Email,
                Phone = student.Phone
            });
        }

        // GET: api/students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _context.Students
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Cedula = s.Cedula,
                    Email = s.Email,
                    Phone = s.Phone
                })
                .ToListAsync();

            return Ok(students);
        }

        // GET: api/students/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentById(int id)
        {
            var student = await _context.Students
                .Include(s => s.Subjects)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            return Ok(new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Cedula = student.Cedula,
                Email = student.Email,
                Phone = student.Phone
            });
        }

        // PUT: api/students/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentCreateDto studentDto)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            student.FirstName = studentDto.FirstName;
            student.LastName = studentDto.LastName;
            student.Cedula = studentDto.Cedula;
            student.Email = studentDto.Email;
            student.Phone = studentDto.Phone;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/students/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound(new { message = "Estudiante no encontrado." });

            var studentSubjects = _context.StudentSubjects.Where(ss => ss.StudentId == id);
            _context.StudentSubjects.RemoveRange(studentSubjects);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
