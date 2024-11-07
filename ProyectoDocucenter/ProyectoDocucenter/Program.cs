using Microsoft.EntityFrameworkCore;
using ProyectoDocucenter.ModelsDB;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Agrega el contexto de base de datos utilizando la cadena de conexión
builder.Services.AddDbContext<BFAContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar servicios adicionales al contenedor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuración del pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es 30 días. Se puede ajustar para producción
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuración de las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
