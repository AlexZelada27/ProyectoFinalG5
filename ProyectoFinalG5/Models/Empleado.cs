using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class Empleado
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEmpleado { get; set; }

        [Required, StringLength(80)]
        public string Nombre { get; set; }

        [Required, StringLength(50)]
        public string Cargo { get; set; }

        [Required]
        public string Correo { get; set; }
    }
}