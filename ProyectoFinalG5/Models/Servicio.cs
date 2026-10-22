using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class Servicio
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdServicio { get; set; }

        [Required, StringLength(80)]
        public string Nombre { get; set; }

        [Required]
        public decimal Precio { get; set; }

        public int IdTipoServicio { get; set; }
        public TipoServicio TipoServicio { get; set; }
    }
}