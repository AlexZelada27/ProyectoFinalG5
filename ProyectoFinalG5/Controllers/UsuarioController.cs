using Microsoft.AspNetCore.Mvc;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using ProyectoFinalG5.ViewModels;
using System.Linq;

namespace ProyectoFinalG5.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;

        public UsuarioController(AppDbContext context)
        {
            _context = context;
        }


        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginVM vm)
        {

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == vm.Email && u.Password == vm.Password);



            if (usuario == null)
            {
                ViewBag.Error = "Usuario o contraseña incorrectos";
                return View(vm);
            }



            HttpContext.Session.SetInt32(
                "IdUsuario",
                usuario.IdUsuario
            );


            HttpContext.Session.SetString(
                "EmailUsuario",
                usuario.Email
            );


            HttpContext.Session.SetString(
                "Rol",
                usuario.IdRol.ToString()
            );





            // ADMINISTRADOR
            if (usuario.IdRol == 1)
            {

                return RedirectToAction(
                    "Index",
                    "Admin"
                );

            }






            // EMPLEADO
            if (usuario.IdRol == 2)
            {

                var empleado = _context.Empleados
                    .FirstOrDefault(e => e.Correo == usuario.Email);


                if (empleado == null)
                {

                    HttpContext.Session.SetString(
                        "PerfilEmpleadoPendiente",
                        "1"
                    );


                    return RedirectToAction(
                        "Nuevo",
                        "Empleado",
                        new { correo = usuario.Email }
                    );

                }


                HttpContext.Session.Remove(
                    "PerfilEmpleadoPendiente"
                );


                return RedirectToAction(
                    "Listar",
                    "Reserva"
                );

            }







            // CLIENTE
            if (usuario.IdRol == 3)
            {

                var cliente = _context.Clientes
                    .FirstOrDefault(c => c.Correo == usuario.Email);


                if (cliente == null)
                {

                    return RedirectToAction(
                        "Nuevo",
                        "Cliente",
                        new { correo = usuario.Email }
                    );

                }


                return RedirectToAction(
                    "Listar",
                    "Reserva"
                );

            }



            return RedirectToAction(
                "Index",
                "Home"
            );
        }
        public IActionResult Registro()
        {
            if (!_context.Roles.Any())
            {
                _context.Roles.AddRange(
                    new Rol { NombreRol = "Administrador" },
                    new Rol { NombreRol = "Empleado" },
                    new Rol { NombreRol = "Cliente" }
                );

                _context.SaveChanges();
            }

            ViewBag.Roles = _context.Roles.ToList();

            var rolPendiente = HttpContext.Session.GetString("RolPendiente");

            if (!string.IsNullOrEmpty(rolPendiente))
            {
                ViewBag.RolSeleccionado = int.Parse(rolPendiente);
            }

            return View();
        }


        [HttpPost]
        public IActionResult Registro(UserVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = _context.Roles.ToList();
                return View(vm);
            }

            var existe = _context.Usuarios.Any(u => u.Email == vm.Email);

            if (existe)
            {
                ViewBag.Error = "El email ya está registrado";
                ViewBag.Roles = _context.Roles.ToList();
                return View(vm);
            }


            if (vm.IdRol == 1)
            {
                var adminValidado = HttpContext.Session.GetString("AdminValidado");

                if (adminValidado != "true")
                {
                    return RedirectToAction("VerificarAdmin", new { rol = vm.IdRol });
                }
            }

            var usuario = new Usuario
            {
                Email = vm.Email,
                Password = vm.Password,
                IdRol = vm.IdRol
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("EmailUsuario", usuario.Email);
            HttpContext.Session.SetString("Rol", usuario.IdRol.ToString());

            HttpContext.Session.Remove("AdminValidado");
            HttpContext.Session.Remove("RolPendiente");

            if (usuario.IdRol == 1)
                return RedirectToAction("Index", "Admin");

            if (usuario.IdRol == 2)
                return RedirectToAction("Nuevo", "Empleado", new { correo = usuario.Email });

            if (usuario.IdRol == 3)
                return RedirectToAction("Nuevo", "Cliente");

            return RedirectToAction("Login");
        }

        public IActionResult VerificarAdmin(int rol)
        {
            HttpContext.Session.SetString("RolPendiente", rol.ToString());
            return View();
        }

        [HttpPost]
        public IActionResult VerificarAdmin(string codigo)
        {
            string codigoCorrecto = "ZONE-ADMIN-2026";

            if (codigo == codigoCorrecto)
            {
                HttpContext.Session.SetString("AdminValidado", "true");
                return RedirectToAction("Registro");
            }

            ViewBag.Error = "Código incorrecto";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Listar()
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            var usuarios = _context.Usuarios.ToList();
            ViewBag.Roles = _context.Roles.ToList();

            return View(usuarios);
        }


        public IActionResult Eliminar(int id)
        {
            var rol = HttpContext.Session.GetString("Rol");

            if (rol != "1")
                return RedirectToAction("Listar", "Reserva");

            var user = _context.Usuarios.Find(id);

            if (user != null)
            {
                _context.Usuarios.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Listar");
        }
    }
}