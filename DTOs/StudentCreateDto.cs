using System.ComponentModel.DataAnnotations;

namespace StudentRegistration.API.DTOs
{
    public class StudentCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar al menos una materia.")]
        [MinLength(1, ErrorMessage = "Debe seleccionar al menos una materia.")]
        [MaxLength(3, ErrorMessage = "Solo puede seleccionar hasta 3 materias.")]
        public List<int> SubjectIds { get; set; } = new();
    }
}
