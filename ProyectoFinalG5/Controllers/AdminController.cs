using Microsoft.AspNetCore.Mvc;

namespace ProyectoFinalG5.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol == null || rol != "1")
            {
                return RedirectToAction("Login", "Usuario");
            }

            return View();
        }
    }
}