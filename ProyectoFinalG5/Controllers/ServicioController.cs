using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;

namespace ProyectoFinalG5.Controllers
{
    public class ServicioController : Controller
    {
        private readonly AppDbContext _context;

        public ServicioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Listar()
        {
            return View(_context.Servicios.ToList());
        }

        public IActionResult Nuevo()
        {
            ViewBag.TipoServicios = _context.TipoServicios.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(Servicio s)
        {
            if (s.IdTipoServicio == 0)
            {
                ViewBag.TipoServicios = _context.TipoServicios.ToList();
                ModelState.AddModelError("", "Seleccione un tipo de servicio");
                return View(s);
            }

            _context.Servicios.Add(s);
            _context.SaveChanges();
            return RedirectToAction("Listar");
        }

        public IActionResult Editar(int id)
        {
            var servicio = _context.Servicios.Find(id);

            if (servicio == null)
                return RedirectToAction("Listar");

            ViewBag.TipoServicios = _context.TipoServicios.ToList();
            return View(servicio);
        }

        [HttpPost]
        public IActionResult Editar(Servicio s)
        {
            var servicio = _context.Servicios.Find(s.IdServicio);

            if (servicio == null)
                return RedirectToAction("Listar");

            if (s.IdTipoServicio == 0)
            {
                ViewBag.TipoServicios = _context.TipoServicios.ToList();
                ModelState.AddModelError("", "Seleccione un tipo de servicio");
                return View(s);
            }

            servicio.Nombre = s.Nombre;
            servicio.Precio = s.Precio;
            servicio.IdTipoServicio = s.IdTipoServicio;

            _context.SaveChanges();

            return RedirectToAction("Listar");
        }

        public IActionResult Eliminar(int id)
        {
            var data = _context.Servicios.Find(id);

            if (data == null)
                return RedirectToAction("Listar");

            _context.Servicios.Remove(data);
            _context.SaveChanges();

            return RedirectToAction("Listar");
        }
    }
}