using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using System;
using System.Linq;

namespace ProyectoFinalG5.Controllers
{
    public class PagoController : Controller
    {
        private readonly AppDbContext _context;

        public PagoController(AppDbContext context)
        {
            _context = context;
        }


        // LISTAR PAGOS
        public IActionResult Listar(DateTime? fecha)
        {
            var pagos = _context.Reservas
                .Include(r => r.Pago)
                .Where(r => r.Pago != null)
                .AsQueryable();


            if (fecha.HasValue)
            {
                pagos = pagos.Where(r => r.FechaReserva.Date == fecha.Value.Date);
            }


            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");


            return View(pagos.Select(r => r.Pago).ToList());
        }



        // DETALLE DE PAGO
        public IActionResult Detalle(int id)
        {
            var pago = _context.Pagos
                .FirstOrDefault(p => p.IdPago == id);


            if (pago == null)
                return RedirectToAction("Listar");


            return View(pago);
        }





        // VISTA NUEVO PAGO
        public IActionResult Nuevo()
        {
            return View();
        }





        // EDITAR PAGO - GET
        public IActionResult Editar(int id)
        {
            var pago = _context.Pagos
                .FirstOrDefault(p => p.IdPago == id);


            if (pago == null)
                return RedirectToAction("Listar");


            return View(pago);
        }

        public IActionResult Crear()
        {
            // Verificar si existe cliente registrado
            var cliente = HttpContext.Session.GetInt32("IdCliente");

            if (cliente == null)
            {
                // Si no está registrado, mandarlo al registro
                return RedirectToAction("Crear", "Cliente");
            }


            Pago pago = new Pago();

            pago.Monto = 100;

            return View(pago);
        }


        // EDITAR PAGO - POST
        [HttpPost]
        public IActionResult Editar(Pago p)
        {
            var pago = _context.Pagos
                .FirstOrDefault(x => x.IdPago == p.IdPago);


            if (pago == null)
                return RedirectToAction("Listar");


            if (!ModelState.IsValid)
                return View(pago);



            pago.Monto = p.Monto;
            pago.MetodoPago = p.MetodoPago;


            _context.SaveChanges();


            return RedirectToAction("Listar");
        }





        // ELIMINAR PAGO
        public IActionResult Eliminar(int id)
        {

            var existeReserva = _context.Reservas
                .Any(r => r.IdPago == id);



            if (existeReserva)
            {
                TempData["Error"] =
                    "No se puede eliminar este pago porque está asociado a una reserva.";

                return RedirectToAction("Listar");
            }



            var pago = _context.Pagos
                .FirstOrDefault(p => p.IdPago == id);



            if (pago == null)
                return RedirectToAction("Listar");



            _context.Pagos.Remove(pago);

            _context.SaveChanges();



            return RedirectToAction("Listar");
        }

    }
}

