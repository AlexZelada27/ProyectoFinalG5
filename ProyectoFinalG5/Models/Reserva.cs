using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class Reserva
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdReserva { get; set; }

        [Required]
        public DateTime FechaReserva { get; set; }

        public int IdCliente { get; set; }
        public Cliente? Cliente { get; set; }

        public int IdEmpleado { get; set; }
        public Empleado? Empleado { get; set; }

        public int IdPago { get; set; }
        public Pago? Pago { get; set; }

        public int IdSala { get; set; }
        public SalaEstudio? SalaEstudio { get; set; }

        public int IdHorarioFijo { get; set; }
        public HorarioFijo? HorarioFijo { get; set; }

        [Required, StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public ICollection<ReservaServicio> ReservaServicios { get; set; }
            = new List<ReservaServicio>();
    }
}