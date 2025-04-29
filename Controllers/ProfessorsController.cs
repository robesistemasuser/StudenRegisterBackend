using Microsoft.AspNetCore.Mvc;
using StudentRegistration.API.Data;
using StudentRegistration.API.DTOs;
using StudentRegistration.API.Models;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfessorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfessor([FromBody] ProfessorCreateDto professorDto)
        {
            if (string.IsNullOrWhiteSpace(professorDto.Name))
                return BadRequest("Name is required.");

            var professor = new Professor
            {
                Name = professorDto.Name
            };

            _context.Professors.Add(professor);
            await _context.SaveChangesAsync();

            return Ok(new ProfessorDto { Id = professor.Id, Name = professor.Name });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetAllProfessors()
        {
            var professors = await _context.Professors
                .Select(p => new ProfessorDto { Id = p.Id, Name = p.Name })
                .ToListAsync();

            return Ok(professors);
        }
    }
}
