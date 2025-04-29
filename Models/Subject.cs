namespace StudentRegistration.API.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ProfessorId { get; set; }

        // Relación con la clase Professor (cada materia tiene un profesor asignado)
        public Professor Professor { get; set; }  

        // Relación con la tabla de unión StudentSubject (relación muchos a muchos entre estudiantes y materias)
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>(); 
    }
}
