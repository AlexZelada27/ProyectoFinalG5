using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class TipoServicio
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTipoServicio { get; set; }

        [Required, StringLength(50)]
        public string Nombre { get; set; }
    }
}