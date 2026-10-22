using Microsoft.EntityFrameworkCore;
using ProyectoFinalG5.Data;
using ProyectoFinalG5.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"));
});

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// ---------------------------------------------------------------------------
// SEMBRADO AUTOMÁTICO DE HORARIOS FIJOS
// ---------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.HorariosFijos.Any())
    {
        context.HorariosFijos.AddRange(
            new HorarioFijo { HoraInicio = new TimeSpan(8, 0, 0), HoraFin = new TimeSpan(10, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(10, 0, 0), HoraFin = new TimeSpan(12, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(12, 0, 0), HoraFin = new TimeSpan(14, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(14, 0, 0), HoraFin = new TimeSpan(16, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(16, 0, 0), HoraFin = new TimeSpan(18, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(18, 0, 0), HoraFin = new TimeSpan(20, 0, 0) },
            new HorarioFijo { HoraInicio = new TimeSpan(20, 0, 0), HoraFin = new TimeSpan(22, 0, 0) }
        );
        context.SaveChanges();
    }
}
// ---------------------------------------------------------------------------

app.Run();