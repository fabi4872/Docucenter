using Microsoft.EntityFrameworkCore;
using ProyectoDocucenter.ModelsDB;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la cadena de conexi�n
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Agrega el contexto de base de datos utilizando la cadena de conexi�n
builder.Services.AddDbContext<BFAContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar servicios adicionales al contenedor
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuraci�n del pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // El valor predeterminado de HSTS es 30 d�as. Se puede ajustar para producci�n
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Configuraci�n de las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
