namespace StudentRegistration.API.DTOs
{
    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public int ProfessorId { get; set; }
        public string ProfessorName { get; set; }

        // Podrías agregar un DTO para el profesor si es necesario en el futuro
        // public ProfessorDto Professor { get; set; }
    }
}
