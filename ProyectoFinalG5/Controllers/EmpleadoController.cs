using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using System.Linq;

namespace ProyectoFinalG5.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly AppDbContext _context;

        public EmpleadoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Nuevo(string correo)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol == null)
                return RedirectToAction("Login", "Usuario");

            if (rol != "2")
                return RedirectToAction("Listar", "Reserva");

            if (string.IsNullOrEmpty(correo))
                correo = HttpContext.Session.GetString("EmailUsuario");

            if (string.IsNullOrEmpty(correo))
                return RedirectToAction("Login", "Usuario");

            var existe = _context.Empleados.Any(e => e.Correo == correo);

            if (existe)
                return RedirectToAction("Listar", "Reserva");

            var model = new Empleado
            {
                Correo = correo
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Nuevo(Empleado e)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "2")
                return RedirectToAction("Listar", "Reserva");

            if (!ModelState.IsValid)
                return View(e);

            var existe = _context.Empleados.Any(x => x.Correo == e.Correo);

            if (existe)
                return RedirectToAction("Listar", "Reserva");

            _context.Empleados.Add(e);
            _context.SaveChanges();

            return RedirectToAction("Listar", "Reserva");
        }

        public IActionResult Listar()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            return View(_context.Empleados.ToList());
        }

        public IActionResult Editar(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return RedirectToAction("Listar", "Reserva");

            return View(empleado);
        }

        [HttpPost]
        public IActionResult Editar(Empleado e)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            var empleado = _context.Empleados.FirstOrDefault(x => x.IdEmpleado == e.IdEmpleado);

            if (empleado == null)
                return RedirectToAction("Listar", "Reserva");

            empleado.Nombre = e.Nombre;
            empleado.Cargo = e.Cargo;

            _context.SaveChanges();

            return RedirectToAction("Listar", "Reserva");
        }

        public IActionResult Eliminar(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            var empleado = _context.Empleados.FirstOrDefault(e => e.IdEmpleado == id);

            if (empleado == null)
                return RedirectToAction("Listar", "Reserva");

            var tieneReservas = _context.Reservas.Any(r => r.IdEmpleado == id);

            if (tieneReservas)
            {
                TempData["Error"] = "No se puede eliminar este empleado porque tiene reservas asignadas.";
                return RedirectToAction("Listar");
            }

            _context.Empleados.Remove(empleado);
            _context.SaveChanges();

            return RedirectToAction("Listar");
        }
    }
}