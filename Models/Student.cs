namespace StudentRegistration.API.Models
{
    public class Student
    {
        public int Id { get; set; }

        // Campos personales
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Relación con materias
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();

        public ICollection<Subject> Subjects => StudentSubjects.Select(ss => ss.Subject).ToList();

        public bool ValidateSubjects() => StudentSubjects.Count <= 3;

        public bool ValidateProfessors()
        {
            var teacherIds = StudentSubjects.Select(ss => ss.Subject.ProfessorId).Distinct().ToList();
            return teacherIds.Count == StudentSubjects.Count;
        }
    }
}
