using Microsoft.EntityFrameworkCore;
using SenaPay.Infrastructure.Data;
using SENA_PAY__PRUEBAS.Middleware;
using SenaPay.Application.UseCases.Aprendiz;
using SenaPay.Application.UseCases.Account;
using SenaPay.Infrastructure.Services;
using SenaPay.Infrastructure.Repositories.Usuarios;
using SenaPay.Application.UseCases.Tienda;
using SenaPay.Infrastructure.Repositories.Tienda;
using SenaPay.Infrastructure.Repositories.Sedes;
using SenaPay.Infrastructure.Repositories;
using SenaPay.Application.UseCases.Sedes;
using SenaPay.Application.UseCases.Usuarios;
using SenaPay.Application.UseCases.Reportes;
using SenaPay.Application.UseCases.RecuperacionContraseña;
using SenaPay.Application.UseCases.Categorias;
using SenaPay.Domain.Interfaces;
using SenaPay.Application.UseCases.AdminTienda;
using System.Text.Json.Serialization;
using SenaPay.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ── MVC + JSON ────────────────────────────────────────────────────────────────
//builder.Services.AddControllersWithViews()
//  .AddJsonOptions(o =>
//    o.JsonSerializerOptions.Converters.Add(
//      new JsonStringEnumConverter()));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Convierte PascalCase C# → camelCase JSON automáticamente
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
        // Enums como string
        options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
    });

// ── Base de Datos ─────────────────────────────────────────────────────────────
builder.Services.AddDbContext<SenaPayContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Antiforgery ───────────────────────────────────────────────────────────────
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "RequestVerificationToken";
});

// ── Sesión (una sola vez) ─────────────────────────────────────────────────────
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(8);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ── Autenticación por Cookies ─────────────────────────────────────────────────
builder.Services.AddAuthentication("SenaPayCookies")
    .AddCookie("SenaPayCookies", options =>
    {
        options.LoginPath = "/Account/Account/Login";
        options.AccessDeniedPath = "/Account/Account/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        // SameAsRequest: HTTPS en producción, HTTP en desarrollo local
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization();

// ── Repositorios ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAprendizRepository, AprendizRepository>();
builder.Services.AddScoped<ITiendaRepository, TiendaRepository>();
builder.Services.AddScoped<ISedeRepository, SedeRepository>();
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ITiendaCategoriaRepository, TiendaCategoriaRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();


// ── Casos de Uso: Aprendiz ────────────────────────────────────────────────────
builder.Services.AddScoped<GetPerfilAprendizUseCase>();
builder.Services.AddScoped<ActualizarPerfilAprendizUseCase>();

// ── Casos de Uso: Account ─────────────────────────────────────────────────────
builder.Services.AddScoped<ValidarAccesoUseCase>();
builder.Services.AddScoped<RecuperarPasswordUseCase>();
builder.Services.AddScoped<VerificarCodigoUseCase>();
builder.Services.AddScoped<RestablecerPasswordUseCase>();

// ── Casos de Uso: Funcionarios / Usuarios ─────────────────────────────────────
builder.Services.AddScoped<AgregarUsuarioUseCase>();
builder.Services.AddScoped<ObtenerUsuariosUseCase>();
builder.Services.AddScoped<ObtenerUsuarioUseCase>();
builder.Services.AddScoped<EditarUsuarioUseCase>();
builder.Services.AddScoped<EliminarUsuarioUseCase>();
builder.Services.AddScoped<CambiarEstadoUsuarioUseCase>();

// ── Casos de Uso: Tiendas (Funcionario) ───────────────────────────────────────
builder.Services.AddScoped<CrearTiendaUseCase>();
builder.Services.AddScoped<ObtenerTiendasUseCase>();
builder.Services.AddScoped<ObtenerSedesUseCase>();
builder.Services.AddScoped<ObtenerAdminsDisponiblesUseCase>();

// ── Casos de Uso: Selección de Tienda (AdminCafeteria) ───────────────────────
builder.Services.AddScoped<ObtenerTiendasDeAdminUseCase>();
builder.Services.AddScoped<SeleccionarTiendaUseCase>();

// ── Casos de Uso: Dashboard y Productos (AdminCafeteria) ──────────────────────
builder.Services.AddScoped<ObtenerDashboardTiendaUseCase>();
builder.Services.AddScoped<GestionarProductoUseCase>();
builder.Services.AddScoped<GestionarCategoriasTiendaUseCase>();
builder.Services.AddScoped<ObtenerProductosUseCase>();
builder.Services.AddScoped<ObtenerDetalleProductoUseCase>();

// ── Casos de Uso: Categorías ──────────────────────────────────────────────────
builder.Services.AddScoped<CrearCategoriaUseCase>();
builder.Services.AddScoped<ObtenerCategoriasUseCase>();
builder.Services.AddScoped<EliminarCategoriaUseCase>();
builder.Services.AddScoped<EditarCategoriaUseCase>();

// ── Casos de Uso: Reportes ────────────────────────────────────────────────────
builder.Services.AddScoped<CrearReporteUseCase>();
builder.Services.AddScoped<ObtenerReportesUseCase>();
builder.Services.AddScoped<CambiarEstadoReporteUseCase>();
builder.Services.AddScoped<ObtenerEstadisticasReportesUseCase>();

// — Servicios de Infraestructura —
builder.Services.AddScoped<IEmailService, EmailService>();

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();
// ─────────────────────────────────────────────────────────────────────────────

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseMiddleware<RoleGuardMiddleware>(); // ← intercepta roles ANTES de los controladores
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Account}/{controller=Account}/{action=Login}/{id?}");

app.Run();