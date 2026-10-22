using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalG5.ViewModels
{
    public class UserVM
    {
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "La contraseña debe tener una mayúscula, un número y un carácter especial")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public int IdRol { get; set; }
    }
}