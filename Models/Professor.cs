namespace StudentRegistration.API.Models
{
    public class Professor
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Relación uno a muchos con Subject (un profesor puede dictar muchas materias)
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}
