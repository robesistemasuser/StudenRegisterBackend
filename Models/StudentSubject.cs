namespace StudentRegistration.API.Models
{
    public class StudentSubject
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }  // Relación con Student

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }  // Relación con Subject
    }
}
