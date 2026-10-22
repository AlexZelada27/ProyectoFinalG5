using System.ComponentModel.DataAnnotations;

namespace ProyectoFinalG5.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}