using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using System.Linq;

namespace ProyectoFinalG5.Controllers
{
    public class RolController : Controller
    {
        private readonly AppDbContext _context;

        public RolController(AppDbContext context)
        {
            _context = context;
        }

        private bool EsAdmin()
        {
            return HttpContext.Session.GetString("Rol") == "1";
        }

        public IActionResult Listar()
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            var roles = _context.Roles.ToList();
            return View(roles);
        }

        public IActionResult Nuevo()
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(Rol r)
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            if (!ModelState.IsValid)
                return View(r);

            _context.Roles.Add(r);
            _context.SaveChanges();

            return RedirectToAction("Listar");
        }

        public IActionResult Editar(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            var rol = _context.Roles.Find(id);

            if (rol == null)
                return RedirectToAction("Listar");

            return View(rol);
        }

        [HttpPost]
        public IActionResult Editar(Rol r)
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            if (!ModelState.IsValid)
                return View(r);

            var rol = _context.Roles.Find(r.IdRol);

            if (rol == null)
                return RedirectToAction("Listar");

            rol.NombreRol = r.NombreRol;

            _context.SaveChanges();

            return RedirectToAction("Listar");
        }

        public IActionResult Eliminar(int id)
        {
            if (!EsAdmin())
                return RedirectToAction("Listar", "Reserva");

            var rol = _context.Roles.Find(id);

            if (rol != null)
            {
                _context.Roles.Remove(rol);
                _context.SaveChanges();
            }

            return RedirectToAction("Listar");
        }
    }
}