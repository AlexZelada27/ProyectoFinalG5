using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;

namespace ProyectoFinalG5.Controllers
{
    public class SalaEstudioController : Controller
    {
        private readonly AppDbContext _context;

        public SalaEstudioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Listar()
        {
            return View(_context.SalaEstudios.ToList());
        }

        public IActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(SalaEstudio s)
        {
            if (string.IsNullOrEmpty(s.Descripcion))
            {
                ModelState.AddModelError("", "La descripción es obligatoria");
                return View(s);
            }

            _context.SalaEstudios.Add(s);
            _context.SaveChanges();
            return RedirectToAction("Listar");
        }

        public IActionResult Editar(int id)
        {
            var sala = _context.SalaEstudios.Find(id);

            if (sala == null)
                return RedirectToAction("Listar");

            return View(sala);
        }

        [HttpPost]
        public IActionResult Editar(SalaEstudio s)
        {
            var sala = _context.SalaEstudios.Find(s.IdSala);

            if (sala == null)
                return RedirectToAction("Listar");

            if (string.IsNullOrEmpty(s.Descripcion))
            {
                ModelState.AddModelError("", "La descripción es obligatoria");
                return View(s);
            }

            sala.Nombre = s.Nombre;
            sala.Direccion = s.Direccion;
            sala.Descripcion = s.Descripcion;
            sala.Capacidad = s.Capacidad;

            _context.SaveChanges();

            return RedirectToAction("Listar");
        }

        public IActionResult Eliminar(int id)
        {
            var data = _context.SalaEstudios.Find(id);

            if (data == null)
                return RedirectToAction("Listar");

            _context.SalaEstudios.Remove(data);
            _context.SaveChanges();

            return RedirectToAction("Listar");
        }
    }
}