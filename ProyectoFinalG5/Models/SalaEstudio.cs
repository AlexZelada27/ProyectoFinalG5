using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class SalaEstudio
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdSala { get; set; }

        [Required, StringLength(80)]
        public string Nombre { get; set; }

        [Required, StringLength(120)]
        public string Direccion { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public int Capacidad { get; set; }
    }
}