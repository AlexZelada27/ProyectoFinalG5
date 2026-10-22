using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ProyectoFinalG5.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;


        public HomeController(AppDbContext context)
        {
            _context = context;
        }



        public IActionResult Index()
        {
            var servicios = _context.Servicios.ToList();

            ViewBag.Servicios = servicios;



            var rutaCanciones = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "canciones"
            );



            if (Directory.Exists(rutaCanciones))
            {

                var canciones = Directory.GetFiles(rutaCanciones)
                    .Select(Path.GetFileName)
                    .ToList();


                ViewBag.Canciones = canciones;

            }
            else
            {

                ViewBag.Canciones = new List<string>();

            }



            return View();
        }






        // PANEL ADMINISTRADOR
        public IActionResult Admin()
        {

            return View();

        }






        // PANEL CLIENTE
        public IActionResult Cliente()
        {

            return View();

        }






        // PANEL EMPLEADO
        public IActionResult Empleado()
        {

            return View();

        }






        public IActionResult Privacy()
        {
            return View();
        }






        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Error()
        {

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });

        }

    }
}