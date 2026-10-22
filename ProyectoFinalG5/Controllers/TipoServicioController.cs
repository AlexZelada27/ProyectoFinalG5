using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;

namespace ProyectoFinalG5.Controllers
{
    public class TipoServicioController : Controller
    {
        private readonly AppDbContext _context;

        public TipoServicioController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Listar()
        {
            return View(_context.TipoServicios.ToList());
        }

        public IActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Nuevo(TipoServicio t)
        {
            _context.TipoServicios.Add(t);
            _context.SaveChanges();
            return RedirectToAction("Listar");
        }

        public IActionResult Editar(int id)
        {
            var tipo = _context.TipoServicios.Find(id);

            if (tipo == null)
                return RedirectToAction("Listar");

            return View(tipo);
        }

        [HttpPost]
        public IActionResult Editar(TipoServicio t)
        {
            _context.TipoServicios.Update(t);
            _context.SaveChanges();
            return RedirectToAction("Listar");
        }

        public IActionResult Eliminar(int id)
        {
            var data = _context.TipoServicios.Find(id);

            if (data == null)
                return RedirectToAction("Listar");

            _context.TipoServicios.Remove(data);
            _context.SaveChanges();

            return RedirectToAction("Listar");
        }
    }
}