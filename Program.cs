using Microsoft.EntityFrameworkCore;
using SenaPay.Infrastructure.Data;
using SenaPay.Domain.Entities;
using SenaPay.Application.UseCases.Aprendiz;
using SenaPay.Application.UseCases.Account;
using SenaPay.Application.UseCases.Funcionarios;
using SenaPay.Infrastructure.Services;
using SenaPay.Domain.Interfaces.Core;
using SenaPay.Domain.Interfaces.Usuarios;
using SenaPay.Infrastructure.Repositories.Usuarios;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Domain.Interfaces.Tienda;
using SenaPay.Infrastructure.Repositories.Tienda;
using SenaPay.Infrastructure.Repositories.Sedes;
using SenaPay.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

// 1. CONFIGURACIÓN DE SERVICIOS (Antes de builder.Build)
builder.Services.AddControllersWithViews();

// Configuración de la Base de Datos
builder.Services.AddDbContext<SenaPayContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAntiforgery(options =>
{
    // El JS enviará el token en este header
    options.HeaderName = "RequestVerificationToken";
});

// Configuración de Sesiones (Indispensable para el Login)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Reemplaza el registro anterior de EmailService si existía
builder.Services.AddScoped<IEmailService, EmailService>();
// Asegúrate que el using apunte a la nueva ubicación:
// using SenaPay.Infrastructure.Services;

// Repositorios (Infraestructura implementa, Dominio define el contrato)
builder.Services.AddScoped<IAprendizRepository, AprendizRepository>();

// Casos de Uso (Aplicación)
builder.Services.AddScoped<GetPerfilAprendizUseCase>();

// ?? Repositorios ??????????????????????????????????????????????
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAprendizRepository, AprendizRepository>(); // del paso anterior

// ?? Casos de Uso: Funcionarios ????????????????????????????????
builder.Services.AddScoped<AgregarUsuarioUseCase>();
builder.Services.AddScoped<ObtenerUsuariosUseCase>();
builder.Services.AddScoped<ObtenerUsuarioUseCase>();
builder.Services.AddScoped<EditarUsuarioUseCase>();
builder.Services.AddScoped<EliminarUsuarioUseCase>();

// ?? Casos de Uso: Account ?????????????????????????????????????
builder.Services.AddScoped<ValidarAccesoUseCase>();
builder.Services.AddScoped<RecuperarPasswordUseCase>();
builder.Services.AddScoped<VerificarCodigoUseCase>();
builder.Services.AddScoped<RestablecerPasswordUseCase>();

// ── Casos de Uso: Tienda ──────────────────────────────────────
builder.Services.AddScoped<AgregarAlCarritoUseCase>();
builder.Services.AddScoped<ProcesarCompraUseCase>();

// ── Repositorios: Tienda ──────────────────────────────────────
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ITransaccionRepository, TransaccionRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();

// ── Sesión (necesaria para el carrito) ────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Casos de Uso: Tienda (agregar a los que ya tenías) ────────
builder.Services.AddScoped<ObtenerProductosUseCase>();
builder.Services.AddScoped<ObtenerDetalleProductoUseCase>();
// ProcesarCompraUseCase y AgregarAlCarritoUseCase ya están registrados

// ── Repositorios nuevos ───────────────────────────────────────
builder.Services.AddScoped<ITiendaRepository, TiendaRepository>();
builder.Services.AddScoped<ISedeRepository, SedeRepository>();

// ── Casos de Uso nuevos ───────────────────────────────────────
builder.Services.AddScoped<CrearTiendaUseCase>();
builder.Services.AddScoped<ObtenerTiendasUseCase>();
builder.Services.AddScoped<ObtenerSedesUseCase>();

// Repositorio
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();

// Caso de uso
builder.Services.AddScoped<CrearReporteUseCase>();

// Casos de uso nuevos
builder.Services.AddScoped<ObtenerReportesUseCase>();
builder.Services.AddScoped<CambiarEstadoReporteUseCase>();
builder.Services.AddScoped<ObtenerEstadisticasReportesUseCase>();

//Esto sirve para cerrar los servicios, luego de esto no se debe colocar mas servicios de aplicacion 
var app = builder.Build();

// 2. CONFIGURACIÓN DEL PIPELINE (Orden de ejecución)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ACTIVAR SESIONES: Debe ir después de Routing y antes de Authorization
app.UseSession();

app.UseAuthorization();

// Agrega ANTES de app.MapControllerRoute
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
// Configuración de la ruta inicial (Login)
//Le decimos al programa que inicie en el controlador Account y que haga la accion Login, ahi el programa se va al Login
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Account}/{controller=Account}/{action=Login}/{id?}");


app.Run();
