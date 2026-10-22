using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class ReservaServicio
    {
        [Key]
        public int IdReservaServicio { get; set; }

        public int IdReserva { get; set; }
        public Reserva Reserva { get; set; }

        public int IdServicio { get; set; }
        public Servicio Servicio { get; set; }

        public decimal Precio { get; set; }
    }
}