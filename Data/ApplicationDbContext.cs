using Microsoft.EntityFrameworkCore;
using StudentRegistration.API.Models;

namespace StudentRegistration.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StudentSubject>()
                .HasKey(ss => new { ss.StudentId, ss.SubjectId }); // <- AQUÍ defines la clave primaria compuesta

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentId);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany()
                .HasForeignKey(ss => ss.SubjectId);

            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Professor)
                .WithMany(p => p.Subjects)
                .HasForeignKey(s => s.ProfessorId);
        }

    }
}
