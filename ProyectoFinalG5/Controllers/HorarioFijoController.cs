using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalG5.Data;

namespace ProyectoFinalG5.Controllers
{
    public class HorarioFijoController : Controller
    {
        private readonly AppDbContext _context;

        public HorarioFijoController(AppDbContext context)
        {
            _context = context;
        }

        // Única función necesaria: Consultar disponibilidad por fecha
        public IActionResult Listar(DateTime? fecha)
        {
            if (fecha == null)
                fecha = DateTime.Today;

            var horarios = _context.HorariosFijos.OrderBy(h => h.HoraInicio).ToList();

            var reservas = _context.Reservas
                .Where(r => r.FechaReserva.Date == fecha.Value.Date)
                .ToList();

            ViewBag.Reservas = reservas;
            ViewBag.Fecha = fecha.Value;

            return View(horarios);
        }
    }
}