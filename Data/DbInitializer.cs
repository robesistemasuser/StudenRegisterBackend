using StudentRegistration.API.Models;

namespace StudentRegistration.API.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Professors.Any() || context.Subjects.Any())
            {
                // Ya hay datos, no volver a insertar
                return;
            }

            // Profesores
            var professors = new Professor[]
            {
                new Professor { Name = "Juan Pérez" },
                new Professor { Name = "María Gómez" },
                new Professor { Name = "Carlos Sánchez" },
                new Professor { Name = "Laura Díaz" },
                new Professor { Name = "Pedro Fernández" }
            };

            context.Professors.AddRange(professors);
            context.SaveChanges();

            // Materias (2 por profesor)
            var subjects = new Subject[]
            {
                new Subject { Name = "Matemáticas Básicas", Credits = 3, ProfessorId = professors[0].Id },
                new Subject { Name = "Álgebra Lineal", Credits = 3, ProfessorId = professors[0].Id },
                new Subject { Name = "Historia Mundial", Credits = 3, ProfessorId = professors[1].Id },
                new Subject { Name = "Literatura Moderna", Credits = 3, ProfessorId = professors[1].Id },
                new Subject { Name = "Programación I", Credits = 3, ProfessorId = professors[2].Id },
                new Subject { Name = "Estructuras de Datos", Credits = 3, ProfessorId = professors[2].Id },
                new Subject { Name = "Biología General", Credits = 3, ProfessorId = professors[3].Id },
                new Subject { Name = "Química Orgánica", Credits = 3, ProfessorId = professors[3].Id },
                new Subject { Name = "Economía Básica", Credits = 3, ProfessorId = professors[4].Id },
                new Subject { Name = "Administración de Empresas", Credits = 3, ProfessorId = professors[4].Id }
            };

            context.Subjects.AddRange(subjects);
            context.SaveChanges();
        }
    }
}
