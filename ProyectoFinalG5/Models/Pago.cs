using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class Pago
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPago { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required, StringLength(30)]
        public string MetodoPago { get; set; }

    }
}