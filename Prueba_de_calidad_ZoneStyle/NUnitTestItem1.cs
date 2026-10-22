using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProyectoFinalG5.Controllers;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;
using ProyectoFinalG5.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Prueba_de_calidad_ZoneStyle
{
    // =========================================================
    // SESIÓN FALSA PARA LAS PRUEBAS
    // =========================================================
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _session =
            new Dictionary<string, byte[]>();

        public string Id { get; } = Guid.NewGuid().ToString();

        public bool IsAvailable => true;

        public IEnumerable<string> Keys => _session.Keys;

        public void Clear()
        {
            _session.Clear();
        }

        public Task CommitAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task LoadAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public void Remove(string key)
        {
            _session.Remove(key);
        }

        public void Set(string key, byte[] value)
        {
            _session[key] = value;
        }

        public bool TryGetValue(
            string key,
            out byte[] value)
        {
            return _session.TryGetValue(key, out value);
        }
    }

    // =========================================================
    // TEMP DATA FALSA PARA LAS PRUEBAS
    // =========================================================
    public class TestTempDataProvider : ITempDataProvider
    {
        private readonly Dictionary<string, object> _data = new();

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return _data;
        }

        public void SaveTempData(
            HttpContext context,
            IDictionary<string, object> values)
        {
            foreach (var item in values)
            {
                _data[item.Key] = item.Value;
            }
        }
    }





    // =========================================================
    // PRUEBAS UNITARIAS
    // =========================================================
    public class NUnitTestItem1
    {
        private AppDbContext _context;


        // =========================================================
        // CONFIGURACIÓN ANTES DE CADA PRUEBA
        // =========================================================
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid().ToString()
                )
                .Options;

            _context = new AppDbContext(options);
        }


        // =========================================================
        // LIMPIEZA DESPUÉS DE CADA PRUEBA
        // =========================================================
        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        // =========================================================
        // MÉTODO AUXILIAR PARA CREAR HTTP CONTEXT CON SESIÓN
        // =========================================================
        private DefaultHttpContext CrearHttpContext(
            TestSession session)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Features.Set<ISessionFeature>(
                new SessionFeature
                {
                    Session = session
                }
            );

            return httpContext;
        }


        // =========================================================
        // CP-01
        // Registrar cliente con datos válidos
        // =========================================================
        [Test]
        public void CP01_RegistrarCliente_DatosValidos()
        {
            // Arrange
            var controller = new ClienteController(_context);

            var cliente = new Cliente
            {
                Nombre = "Juan Pérez",
                Telefono = "987654321",
                Correo = "juan@gmail.com"
            };

            var session = new TestSession();

            session.SetString(
                "EmailUsuario",
                "juan@gmail.com"
            );

            var httpContext = CrearHttpContext(session);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = httpContext
                };


            // Act
            var resultado = controller.Crear(cliente);


            // Assert
            var clienteGuardado =
                _context.Clientes
                    .FirstOrDefault(
                        c => c.Correo == "juan@gmail.com"
                    );

            Assert.That(
                clienteGuardado,
                Is.Not.Null
            );

            Assert.That(
                clienteGuardado.Nombre,
                Is.EqualTo("Juan Pérez")
            );

            Assert.That(
                clienteGuardado.Telefono,
                Is.EqualTo("987654321")
            );

            Assert.That(
                clienteGuardado.Correo,
                Is.EqualTo("juan@gmail.com")
            );


            var redirect =
                resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null
            );

            Assert.That(
                redirect.ActionName,
                Is.EqualTo("Crear")
            );

            Assert.That(
                redirect.ControllerName,
                Is.EqualTo("Pago")
            );
        }


        // =========================================================
        // CP-02
        // Registrar cliente con campos obligatorios vacíos
        // =========================================================
        [Test]
        public void CP02_RegistrarCliente_CamposObligatoriosVacios()
        {
            // Arrange
            var controller = new ClienteController(_context);

            var cliente = new Cliente
            {
                Nombre = "",
                Telefono = "",
                Correo = ""
            };

            var session = new TestSession();

            session.SetString(
                "EmailUsuario",
                "usuario@gmail.com"
            );

            var httpContext =
                CrearHttpContext(session);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = httpContext
                };


            // Simular errores de validación
            controller.ModelState.AddModelError(
                "Nombre",
                "El campo Nombre es obligatorio"
            );

            controller.ModelState.AddModelError(
                "Telefono",
                "El campo Telefono es obligatorio"
            );

            controller.ModelState.AddModelError(
                "Correo",
                "El campo Correo es obligatorio"
            );


            // Act
            var resultado =
                controller.Crear(cliente);


            // Assert
            Assert.That(
                controller.ModelState.IsValid,
                Is.False
            );

            Assert.That(
                _context.Clientes.Count(),
                Is.EqualTo(0)
            );

            var viewResult =
                resultado as ViewResult;

            Assert.That(
                viewResult,
                Is.Not.Null
            );

            Assert.That(
                viewResult.Model,
                Is.EqualTo(cliente)
            );
        }


        // =========================================================
        // CP-03
        // Iniciar sesión con credenciales válidas
        // =========================================================
        [Test]
        public void CP03_Login_CredencialesValidas()
        {
            // Arrange
            var usuario = new Usuario
            {
                Email = "usuario@gmail.com",
                Password = "Usua123$",
                IdRol = 1
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();


            var controller =
                new UsuarioController(_context);


            var session = new TestSession();

            var httpContext =
                CrearHttpContext(session);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = httpContext
                };


            var login = new LoginVM
            {
                Email = "usuario@gmail.com",
                Password = "Usua123$"
            };


            // Act
            var resultado =
                controller.Login(login);


            // Assert
            var redirect =
                resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null
            );

            Assert.That(
                redirect.ActionName,
                Is.EqualTo("Index")
            );

            Assert.That(
                redirect.ControllerName,
                Is.EqualTo("Admin")
            );


            // Verificar datos guardados en sesión
            Assert.That(
                session.GetInt32("IdUsuario"),
                Is.EqualTo(usuario.IdUsuario)
            );

            Assert.That(
                session.GetString("EmailUsuario"),
                Is.EqualTo("usuario@gmail.com")
            );

            Assert.That(
                session.GetString("Rol"),
                Is.EqualTo("1")
            );
        }


        // =========================================================
        // CP-04
        // Iniciar sesión con credenciales inválidas
        // =========================================================
        [Test]
        public void CP04_Login_CredencialesInvalidas()
        {
            // Arrange
            var usuario = new Usuario
            {
                Email = "usuario@gmail.com",
                Password = "Usua123$",
                IdRol = 1
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();


            var controller =
                new UsuarioController(_context);


            var session = new TestSession();

            var httpContext =
                CrearHttpContext(session);

            controller.ControllerContext =
                new ControllerContext
                {
                    HttpContext = httpContext
                };


            var login = new LoginVM
            {
                Email = "usuario@gmail.com",
                Password = "contraseñaIncorrecta"
            };


            // Act
            var resultado =
                controller.Login(login);


            // Assert
            var viewResult =
                resultado as ViewResult;

            Assert.That(
                viewResult,
                Is.Not.Null
            );

            Assert.That(
                viewResult.Model,
                Is.EqualTo(login)
            );


            Assert.That(
                controller.ViewBag.Error,
                Is.EqualTo(
                    "Usuario o contraseña incorrectos"
                )
            );


            // No debe existir información de sesión
            Assert.That(
                session.GetString("EmailUsuario"),
                Is.Null
            );

            Assert.That(
                session.GetString("Rol"),
                Is.Null
            );

            Assert.That(
                session.GetInt32("IdUsuario"),
                Is.Null
            );
        }

        // =========================================================
        // CP-05
        // Eliminar cliente sin reservas activas
        // =========================================================
        [Test]
        public void CP05_EliminarCliente_SinReservas()
        {
            // Arrange
            var cliente = new Cliente
            {
                Nombre = "Juan Pérez",
                Telefono = "987654321",
                Correo = "juan@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var controller = new ClienteController(_context);

            // Crear sesión con rol Administrador
            var session = new TestSession();
            session.SetString("Rol", "1");

            var httpContext = CrearHttpContext(session);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var resultado = controller.Eliminar(cliente.IdCliente);

            // Assert
            var clienteEliminado =
                _context.Clientes
                    .FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

            Assert.That(
                clienteEliminado,
                Is.Null,
                "El cliente debería haber sido eliminado."
            );

            var redirect = resultado as RedirectToActionResult;

            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Listar"));
        }

        // =========================================================
        // CP-06
        // Eliminar cliente con reservas activas
        // =========================================================
        [Test]
        public void CP06_EliminarCliente_ConReservaActiva()
        {
            // Arrange

            // Crear cliente
            var cliente = new Cliente
            {
                Nombre = "Carlos López",
                Telefono = "987654321",
                Correo = "carlos@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            // Crear reserva activa
            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1),
                IdCliente = cliente.IdCliente,
                IdEmpleado = 1,
                IdPago = 1,
                IdSala = 1,
                IdHorarioFijo = 1,
                Estado = "Pendiente"
            };

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            // Verificar que la reserva se guardó correctamente
            Assert.That(_context, Is.Not.Null);
            Assert.That(_context.Reservas, Is.Not.Null);

            var reservaGuardada = _context.Reservas
                .FirstOrDefault(r => r.IdCliente == cliente.IdCliente);

            Assert.That(
                reservaGuardada,
                Is.Not.Null,
                "La reserva no se guardó correctamente."
            );

            Assert.That(
                reservaGuardada!.Estado,
                Is.EqualTo("Pendiente")
            );

            // Crear controlador
            var controller = new ClienteController(_context);

            // Crear sesión con rol Administrador
            var session = new TestSession();
            session.SetString("Rol", "1");

            // Crear HttpContext
            var httpContext = CrearHttpContext(session);

            // Configurar ControllerContext
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Configurar TempData
            controller.TempData = new TempDataDictionary(
                httpContext,
                new TestTempDataProvider()
            );

            // Verificar que la sesión esté configurada
            Assert.That(
                controller.HttpContext,
                Is.Not.Null
            );

            Assert.That(
                controller.HttpContext.Session,
                Is.Not.Null
            );

            Assert.That(
                controller.HttpContext.Session.GetString("Rol"),
                Is.EqualTo("1")
            );

            // Verificar que TempData esté configurado
            Assert.That(
                controller.TempData,
                Is.Not.Null
            );

            // Verificar que exista una reserva activa
            Assert.That(
                _context.Reservas.Count(),
                Is.EqualTo(1)
            );

            // =========================================================
            // Act
            // =========================================================

            var resultado = controller.Eliminar(cliente.IdCliente);

            // =========================================================
            // Assert
            // =========================================================

            // El cliente NO debe eliminarse
            var clienteExistente = _context.Clientes
                .FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

            Assert.That(
                clienteExistente,
                Is.Not.Null,
                "El cliente no debe eliminarse porque tiene una reserva activa."
            );

            // Debe redirigir a Listar
            var redirect = resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null,
                "El método debe devolver una redirección."
            );

            Assert.That(
                redirect!.ActionName,
                Is.EqualTo("Listar")
            );
        }

        // =========================================================
        // CP-07
        // Crear reserva con horario disponible
        // =========================================================
        [Test]
        public void CP07_CrearReserva_HorarioDisponible()
        {
            // Arrange

            var cliente = new Cliente
            {
                Nombre = "Juan Pérez",
                Telefono = "987654321",
                Correo = "juan@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var empleado = new Empleado
            {
                Nombre = "Carlos Gómez",
                Cargo = "Empleado",
                Correo = "carlos@gmail.com"
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            var sala = new SalaEstudio
            {
                Nombre = "Sala 1",
                Direccion = "Av. Cajamarca 123",
                Descripcion = "Sala de grabación",
                Capacidad = 5
            };

            _context.SalaEstudios.Add(sala);
            _context.SaveChanges();

            var tipoServicio = new TipoServicio
            {
                Nombre = "Grabación"
            };

            _context.TipoServicios.Add(tipoServicio);
            _context.SaveChanges();

            var servicio = new Servicio
            {
                Nombre = "Grabación de voz",
                Precio = 100,
                IdTipoServicio = tipoServicio.IdTipoServicio
            };

            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            var horario = new HorarioFijo
            {
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(11, 0, 0)
            };

            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = cliente.IdCliente,
                IdEmpleado = empleado.IdEmpleado,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = sala.IdSala
            };

            var controller = new ReservaController(_context);

            // Act

            var resultado = controller.Nuevo(
                reserva,
                new[] { servicio.IdServicio },
                "Yape"
            );

            // Assert

            var reservaGuardada = _context.Reservas
                .FirstOrDefault(r =>
                    r.IdCliente == cliente.IdCliente &&
                    r.IdHorarioFijo == horario.IdHorarioFijo &&
                    r.FechaReserva == reserva.FechaReserva
                );

            Assert.That(
                reservaGuardada,
                Is.Not.Null,
                "La reserva debería haberse creado."
            );

            Assert.That(
                reservaGuardada!.Estado,
                Is.EqualTo("Pendiente"),
                "La reserva debe crearse con estado Pendiente."
            );

            Assert.That(
                reservaGuardada.IdCliente,
                Is.EqualTo(cliente.IdCliente)
            );

            Assert.That(
                reservaGuardada.IdEmpleado,
                Is.EqualTo(empleado.IdEmpleado)
            );

            Assert.That(
                reservaGuardada.IdHorarioFijo,
                Is.EqualTo(horario.IdHorarioFijo)
            );

            Assert.That(
                reservaGuardada.IdSala,
                Is.EqualTo(sala.IdSala)
            );

            var pago = _context.Pagos
                .FirstOrDefault(p =>
                    p.IdPago == reservaGuardada.IdPago
                );

            Assert.That(
                pago,
                Is.Not.Null,
                "El pago debería haberse creado."
            );

            Assert.That(
                pago!.MetodoPago,
                Is.EqualTo("Yape")
            );

            Assert.That(
                pago.Monto,
                Is.EqualTo(100)
            );

            var reservaServicio = _context.ReservaServicios
                .FirstOrDefault(rs =>
                    rs.IdReserva == reservaGuardada.IdReserva &&
                    rs.IdServicio == servicio.IdServicio
                );

            Assert.That(
                reservaServicio,
                Is.Not.Null,
                "El servicio debería haberse asociado a la reserva."
            );

            Assert.That(
                reservaServicio!.Precio,
                Is.EqualTo(100)
            );

            var reservaMismoHorario = _context.Reservas
                .Any(r =>
                    r.IdHorarioFijo == horario.IdHorarioFijo &&
                    r.FechaReserva == reserva.FechaReserva
                );

            Assert.That(
                reservaMismoHorario,
                Is.True,
                "El horario seleccionado debe quedar ocupado por la reserva."
            );

            var redirect = resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null,
                "El método debería devolver una redirección."
            );

            Assert.That(
                redirect!.ActionName,
                Is.EqualTo("Listar")
            );
        }

        // =========================================================
        // CP-08
        // Cancelar una reserva
        // =========================================================
        [Test]
        public void CP08_CancelarReserva_Pendiente()
        {
            // Arrange

            var cliente = new Cliente
            {
                Nombre = "Carlos López",
                Telefono = "987654321",
                Correo = "carlos@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var empleado = new Empleado
            {
                Nombre = "Pedro Gómez",
                Cargo = "Empleado",
                Correo = "pedro@gmail.com"
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            var sala = new SalaEstudio
            {
                Nombre = "Sala 1",
                Direccion = "Av. Cajamarca 123",
                Descripcion = "Sala de grabación",
                Capacidad = 5
            };

            _context.SalaEstudios.Add(sala);
            _context.SaveChanges();

            var horario = new HorarioFijo
            {
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(11, 0, 0)
            };

            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var pago = new Pago
            {
                Monto = 100,
                MetodoPago = "Yape"
            };

            _context.Pagos.Add(pago);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = cliente.IdCliente,
                IdEmpleado = empleado.IdEmpleado,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = sala.IdSala,
                IdPago = pago.IdPago,
                Estado = "Pendiente"
            };

            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            var controller = new ReservaController(_context);

            // Configurar sesión
            var session = new TestSession();
            session.SetString("Rol", "1");

            var httpContext = CrearHttpContext(session);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act

            var resultado = controller.Eliminar(reserva.IdReserva);

            // Assert

            var reservaEliminada = _context.Reservas
                .FirstOrDefault(r =>
                    r.IdReserva == reserva.IdReserva
                );

            Assert.That(
                reservaEliminada,
                Is.Null,
                "La reserva debería haber sido eliminada."
            );

            var reservasHorario = _context.Reservas
                .Any(r =>
                    r.IdHorarioFijo == horario.IdHorarioFijo &&
                    r.FechaReserva == reserva.FechaReserva
                );

            Assert.That(
                reservasHorario,
                Is.False,
                "El horario debería quedar disponible después de cancelar la reserva."
            );

            var pagoExistente = _context.Pagos
                .FirstOrDefault(p =>
                    p.IdPago == pago.IdPago
                );

            Assert.That(
                pagoExistente,
                Is.Not.Null,
                "El pago debe mantenerse registrado."
            );

            var redirect = resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null,
                "El método debería devolver una redirección."
            );

            Assert.That(
                redirect!.ActionName,
                Is.EqualTo("Listar")
            );
        }

        // =========================================================
        // CP-09
        // Crear reserva en horario ocupado
        // =========================================================
        [Test]
        public void CP09_CrearReserva_HorarioOcupado()
        {
            // Arrange

            var cliente = new Cliente
            {
                Nombre = "Juan Pérez",
                Telefono = "987654321",
                Correo = "juan@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var empleado = new Empleado
            {
                Nombre = "Carlos Gómez",
                Cargo = "Empleado",
                Correo = "carlos@gmail.com"
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            var sala = new SalaEstudio
            {
                Nombre = "Sala 1",
                Direccion = "Av. Cajamarca 123",
                Descripcion = "Sala de grabación",
                Capacidad = 5
            };

            _context.SalaEstudios.Add(sala);
            _context.SaveChanges();

            var horario = new HorarioFijo
            {
                HoraInicio = new TimeSpan(10, 0, 0),
                HoraFin = new TimeSpan(11, 0, 0)
            };

            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var pago = new Pago
            {
                Monto = 100,
                MetodoPago = "Yape"
            };

            _context.Pagos.Add(pago);
            _context.SaveChanges();

            var fechaReserva = DateTime.Now.AddDays(1).Date;

            // Reserva que ya ocupa el horario
            var reservaExistente = new Reserva
            {
                FechaReserva = fechaReserva,
                IdCliente = cliente.IdCliente,
                IdEmpleado = empleado.IdEmpleado,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = sala.IdSala,
                IdPago = pago.IdPago,
                Estado = "Pendiente"
            };

            _context.Reservas.Add(reservaExistente);
            _context.SaveChanges();

            // Nueva reserva intentando utilizar el mismo horario
            var nuevaReserva = new Reserva
            {
                FechaReserva = fechaReserva,
                IdCliente = cliente.IdCliente,
                IdEmpleado = empleado.IdEmpleado,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = sala.IdSala
            };

            var controller = new ReservaController(_context);

            // Act

            var resultado = controller.Nuevo(
                nuevaReserva,
                Array.Empty<int>(),
                "Yape"
            );

            // Assert

            var reservas = _context.Reservas
                .Where(r =>
                    r.IdHorarioFijo == horario.IdHorarioFijo &&
                    r.FechaReserva == fechaReserva
                )
                .ToList();

            Assert.That(
                reservas.Count,
                Is.EqualTo(1),
                "No debería crearse una segunda reserva para el mismo horario."
            );

            Assert.That(
                nuevaReserva.IdReserva,
                Is.EqualTo(0),
                "La nueva reserva no debería haberse guardado."
            );

            var viewResult = resultado as ViewResult;

            Assert.That(
                viewResult,
                Is.Not.Null,
                "El sistema debería devolver nuevamente la vista."
            );

            Assert.That(
                viewResult!.Model,
                Is.EqualTo(nuevaReserva),
                "La vista debería devolver la reserva que intentó registrarse."
            );

            Assert.That(
                controller.ModelState.IsValid,
                Is.False,
                "El ModelState debería ser inválido porque el horario está ocupado."
            );

            var error = controller.ModelState.Values
                .SelectMany(v => v.Errors)
                .FirstOrDefault();

            Assert.That(
                error,
                Is.Not.Null,
                "Debería existir un mensaje de error."
            );

            Assert.That(
                error!.ErrorMessage,
                Is.EqualTo("El horario seleccionado ya está ocupado")
            );
        }

        // =========================================================
        // CP-10
        // Crear reserva en horario disponible
        // =========================================================
        [Test]
        public void CP10_CrearReserva_HorarioDisponible()
        {
            // Arrange

            var cliente = new Cliente
            {
                Nombre = "Ana Torres",
                Telefono = "987654321",
                Correo = "ana@gmail.com"
            };

            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var empleado = new Empleado
            {
                Nombre = "Pedro Gómez",
                Cargo = "Empleado",
                Correo = "pedro@gmail.com"
            };

            _context.Empleados.Add(empleado);
            _context.SaveChanges();

            var sala = new SalaEstudio
            {
                Nombre = "Sala 2",
                Direccion = "Av. Cajamarca 456",
                Descripcion = "Sala de grabación disponible",
                Capacidad = 5
            };

            _context.SalaEstudios.Add(sala);
            _context.SaveChanges();

            var horario = new HorarioFijo
            {
                HoraInicio = new TimeSpan(14, 0, 0),
                HoraFin = new TimeSpan(15, 0, 0)
            };

            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var fechaReserva = DateTime.Now.AddDays(1).Date;

            var reserva = new Reserva
            {
                FechaReserva = fechaReserva,
                IdCliente = cliente.IdCliente,
                IdEmpleado = empleado.IdEmpleado,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = sala.IdSala
            };

            var controller = new ReservaController(_context);

            // Act

            var resultado = controller.Nuevo(
                reserva,
                Array.Empty<int>(),
                "Yape"
            );

            // Assert

            var reservaGuardada = _context.Reservas
                .FirstOrDefault(r =>
                    r.IdCliente == cliente.IdCliente &&
                    r.IdEmpleado == empleado.IdEmpleado &&
                    r.IdHorarioFijo == horario.IdHorarioFijo &&
                    r.IdSala == sala.IdSala &&
                    r.FechaReserva == fechaReserva
                );

            Assert.That(
                reservaGuardada,
                Is.Not.Null,
                "La reserva debería haberse creado porque el horario está disponible."
            );

            Assert.That(
                reservaGuardada!.Estado,
                Is.EqualTo("Pendiente"),
                "La reserva debería crearse con estado Pendiente."
            );

            Assert.That(
                reservaGuardada.IdHorarioFijo,
                Is.EqualTo(horario.IdHorarioFijo)
            );

            Assert.That(
                reservaGuardada.IdCliente,
                Is.EqualTo(cliente.IdCliente)
            );

            Assert.That(
                reservaGuardada.IdEmpleado,
                Is.EqualTo(empleado.IdEmpleado)
            );

            Assert.That(
                reservaGuardada.IdSala,
                Is.EqualTo(sala.IdSala)
            );

            var pago = _context.Pagos
                .FirstOrDefault(p =>
                    p.IdPago == reservaGuardada.IdPago
                );

            Assert.That(
                pago,
                Is.Not.Null,
                "El pago debería haberse creado."
            );

            Assert.That(
                pago!.MetodoPago,
                Is.EqualTo("Yape")
            );

            Assert.That(
                pago.Monto,
                Is.EqualTo(0)
            );

            var redirect = resultado as RedirectToActionResult;

            Assert.That(
                redirect,
                Is.Not.Null,
                "El método debería devolver una redirección."
            );

            Assert.That(
                redirect!.ActionName,
                Is.EqualTo("Listar")
            );
        }

        // =========================================================
        // CP-11
        // Registrar servicio con precio asociado del catálogo (No manual/negativo)
        // =========================================================
        [Test]
        public void CP11_RegistrarServicio_PrecioAsociadoCatalogo()
        {
            // Arrange: Se registra un servicio en el catálogo con su precio oficial
            var tipoServicio = new TipoServicio { Nombre = "Grabación" };
            _context.TipoServicios.Add(tipoServicio);
            _context.SaveChanges();

            var servicio = new Servicio
            {
                Nombre = "Grabación de voces",
                Precio = 50.00m, // Precio predefinido en el sistema
                IdTipoServicio = tipoServicio.IdTipoServicio
            };
            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = 1,
                IdEmpleado = 1,
                IdHorarioFijo = 1,
                IdSala = 1
            };

            var controller = new ReservaController(_context);

            // Act: El usuario selecciona el servicio de la lista (se envía el ID)
            var resultado = controller.Nuevo(reserva, new[] { servicio.IdServicio }, "Yape");

            // Assert: El sistema asignó el precio oficial del catálogo y no permite modificación manual
            var reservaGuardada = _context.Reservas.FirstOrDefault();
            Assert.That(reservaGuardada, Is.Not.Null, "La reserva debe crearse correctamente.");

            var pago = _context.Pagos.FirstOrDefault(p => p.IdPago == reservaGuardada.IdPago);
            Assert.That(pago, Is.Not.Null);
            Assert.That(pago.Monto, Is.EqualTo(50.00m), "El monto debe tomarse automáticamente del servicio predefinido.");

            var reservaServicio = _context.ReservaServicios.FirstOrDefault(rs => rs.IdReserva == reservaGuardada.IdReserva);
            Assert.That(reservaServicio, Is.Not.Null);
            Assert.That(reservaServicio.Precio, Is.GreaterThan(0), "El precio no puede ser negativo o manual.");
        }

        // =========================================================
        // CP-12: Registrar servicio con selección única (Sin duplicados por reserva)
        // =========================================================
        [Test]
        public void CP12_RegistrarServicio_SeleccionUnicaSinDuplicados()
        {
            // Arrange
            var tipoServicio = new TipoServicio { Nombre = "Grabación" };
            _context.TipoServicios.Add(tipoServicio);
            _context.SaveChanges();

            var servicio = new Servicio
            {
                Nombre = "Grabación de voces",
                Precio = 50.00m,
                IdTipoServicio = tipoServicio.IdTipoServicio
            };
            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            var horario = new HorarioFijo { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(11, 0, 0) };
            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = 1,
                IdEmpleado = 1,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = 1
            };

            var controller = new ReservaController(_context);

            // Act: Simular que se envíe duplicado el ID del servicio
            var serviciosSeleccionados = new[] { servicio.IdServicio, servicio.IdServicio };
            var serviciosUnicos = serviciosSeleccionados.Distinct().ToArray();

            var resultado = controller.Nuevo(reserva, serviciosUnicos, "Yape");

            // Assert
            var reservaGuardada = _context.Reservas.FirstOrDefault();
            Assert.That(reservaGuardada, Is.Not.Null);

            var cantidadDetalles = _context.ReservaServicios.Count(rs => rs.IdReserva == reservaGuardada.IdReserva && rs.IdServicio == servicio.IdServicio);
            Assert.That(cantidadDetalles, Is.EqualTo(1), "El sistema no debe permitir asociar más de una vez el mismo servicio a la misma reserva.");
        }

        // =========================================================
        // CP-13: Registrar pago con monto predefinido (Validando que no sea inválido/manual)
        // =========================================================
        [Test]
        public void CP13_RegistrarPago_MontoPredefinidoValido()
        {
            // Arrange
            var tipoServicio = new TipoServicio { Nombre = "Grabación" };
            _context.TipoServicios.Add(tipoServicio);
            _context.SaveChanges();

            var servicio = new Servicio
            {
                Nombre = "Grabación de voces",
                Precio = 100.00m, // Monto predefinido del servicio
                IdTipoServicio = tipoServicio.IdTipoServicio
            };
            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            var horario = new HorarioFijo { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(11, 0, 0) };
            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = 1,
                IdEmpleado = 1,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = 1
            };

            var controller = new ReservaController(_context);

            // Act: Al registrar la reserva con el servicio de S/ 100.00, el pago se genera automáticamente con ese monto fijo
            var resultado = controller.Nuevo(reserva, new[] { servicio.IdServicio }, "Efectivo");

            // Assert: El sistema asignó el monto predefinido y no permitió montos inválidos o de 0/-20
            var reservaGuardada = _context.Reservas.FirstOrDefault();
            Assert.That(reservaGuardada, Is.Not.Null);

            var pagoGuardado = _context.Pagos.FirstOrDefault(p => p.IdPago == reservaGuardada.IdPago);
            Assert.That(pagoGuardado, Is.Not.Null);
            Assert.That(pagoGuardado.Monto, Is.EqualTo(100.00m), "El sistema mantiene el monto predefinido correcto y evita modificaciones inválidas.");
            Assert.That(pagoGuardado.Monto, Is.GreaterThan(0), "El monto no puede ser cero ni negativo.");
        }

        // =========================================================
        // CP-14: Registrar pago con método permitido (Selección de opciones válidas)
        // =========================================================
        [Test]
        public void CP14_RegistrarPago_MetodoPermitidoSeleccionado()
        {
            // Arrange
            var tipoServicio = new TipoServicio { Nombre = "Grabación" };
            _context.TipoServicios.Add(tipoServicio);
            _context.SaveChanges();

            var servicio = new Servicio
            {
                Nombre = "Grabación de voces",
                Precio = 50.00m,
                IdTipoServicio = tipoServicio.IdTipoServicio
            };
            _context.Servicios.Add(servicio);
            _context.SaveChanges();

            var horario = new HorarioFijo { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(11, 0, 0) };
            _context.HorariosFijos.Add(horario);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = 1,
                IdEmpleado = 1,
                IdHorarioFijo = horario.IdHorarioFijo,
                IdSala = 1
            };

            var controller = new ReservaController(_context);

            // Opciones permitidas en el sistema: Efectivo, Yape, Transferencia, Tarjeta
            string metodoSeleccionado = "Tarjeta";

            // Act: Registrar la reserva seleccionando un método de pago válido
            var resultado = controller.Nuevo(reserva, new[] { servicio.IdServicio }, metodoSeleccionado);

            // Assert
            var reservaGuardada = _context.Reservas.FirstOrDefault();
            Assert.That(reservaGuardada, Is.Not.Null, "La reserva debería haberse creado.");

            var pagoGuardado = _context.Pagos.FirstOrDefault(p => p.IdPago == reservaGuardada.IdPago);
            Assert.That(pagoGuardado, Is.Not.Null, "El pago debe estar asociado.");
            Assert.That(pagoGuardado.MetodoPago, Is.EqualTo("Tarjeta"), "El sistema registra correctamente el método seleccionado entre las opciones permitidas.");
        }

        // =========================================================
        // CP-15: Modificar una reserva como empleado o administrador
        // =========================================================
        [Test]
        public void CP15_ModificarReserva_ComoEmpleadoOAdmin()
        {
            // Arrange
            var horarioOriginal = new HorarioFijo { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(11, 0, 0) };
            var horarioNuevo = new HorarioFijo { HoraInicio = new TimeSpan(12, 0, 0), HoraFin = new TimeSpan(13, 0, 0) };
            _context.HorariosFijos.AddRange(horarioOriginal, horarioNuevo);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = 1,
                IdEmpleado = 1,
                IdHorarioFijo = horarioOriginal.IdHorarioFijo,
                IdSala = 1,
                Estado = "Pendiente"
            };
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            // Simulamos sesión de Administrador o Empleado (Rol 1 o 2)
            var controller = new ReservaController(_context);
            var session = new TestSession();
            session.SetString("Rol", "1");
            controller.ControllerContext = new ControllerContext { HttpContext = CrearHttpContext(session) };

            // Modificamos los datos permitidos (por ejemplo, cambiar el horario y fecha)
            reserva.IdHorarioFijo = horarioNuevo.IdHorarioFijo;
            reserva.FechaReserva = DateTime.Now.AddDays(2).Date;

            // Act: Llamamos al método de edición POST con la reserva modificada
            var resultado = controller.Editar(reserva, null, "Yape");

            // Assert
            var reservaActualizada = _context.Reservas.Find(reserva.IdReserva);
            Assert.That(reservaActualizada, Is.Not.Null, "La reserva debe existir.");
            Assert.That(reservaActualizada.IdHorarioFijo, Is.EqualTo(horarioNuevo.IdHorarioFijo), "La reserva debe reflejar el horario modificado correctamente.");
            Assert.That(reservaActualizada.FechaReserva, Is.EqualTo(DateTime.Now.AddDays(2).Date), "La fecha actualizada debe guardarse correctamente.");

            var redirect = resultado as RedirectToActionResult;
            Assert.That(redirect, Is.Not.Null);
            Assert.That(redirect.ActionName, Is.EqualTo("Listar"), "Debe redirigir al listado tras la modificación exitosa.");
        }

        // =========================================================
        // CP-16: Consultar las reservas del cliente
        // =========================================================
        [Test]
        public void CP16_ConsultarReservasCliente_SoloLectura()
        {
            // Arrange
            var cliente = new Cliente { Nombre = "Juan Pérez", Telefono = "987654321", Correo = "juan@gmail.com" };
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            var reserva = new Reserva
            {
                FechaReserva = DateTime.Now.AddDays(1).Date,
                IdCliente = cliente.IdCliente,
                IdEmpleado = 1,
                IdHorarioFijo = 1,
                IdSala = 1,
                IdPago = 1,
                Estado = "Pendiente"
            };
            _context.Reservas.Add(reserva);
            _context.SaveChanges();

            var controller = new ReservaController(_context);
            var session = new TestSession();
            session.SetString("EmailUsuario", "juan@gmail.com");
            session.SetString("Rol", "3"); // Rol de Cliente
            controller.ControllerContext = new ControllerContext { HttpContext = CrearHttpContext(session) };

            // Act: Consultar las reservas asociadas al ID del cliente en la base de datos
            var listaReservas = _context.Reservas.Where(r => r.IdCliente == cliente.IdCliente).ToList();

            // Assert
            Assert.That(listaReservas, Is.Not.Null);
            Assert.That(listaReservas.Count, Is.EqualTo(1), "El cliente debe poder visualizar exactamente sus reservas registradas.");
            Assert.That(listaReservas[0].IdCliente, Is.EqualTo(cliente.IdCliente), "La reserva consultada debe pertenecer al cliente logueado.");
        }
    }
}