using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinalG5.Models
{
    public class HorarioFijo
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHorarioFijo { get; set; }

        [Required]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        public TimeSpan HoraFin { get; set; }

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}