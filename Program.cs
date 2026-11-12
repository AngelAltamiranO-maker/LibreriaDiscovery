using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models; 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// Configuración de la conexión a SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession();

// Configurar Autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// --- BLOQUE PARA CREAR LA BASE DE DATOS Y DATOS INICIALES ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated(); // Crea el archivo libreria.db y las tablas

        // Insertar libros de prueba si la tabla está vacía
        if (!context.Libros.Any())
        {
            context.Libros.AddRange(
                new Libro { Titulo = "Cien años de soledad", Autor = "Gabriel García Márquez", Precio = 299.00m, Stock = 10, ImagenUrl = "https://images.unsplash.com/photo-1544716278-ca5e3f4abd8c" },
                new Libro { Titulo = "Don Quijote de la Mancha", Autor = "Miguel de Cervantes", Precio = 350.00m, Stock = 5, ImagenUrl = "https://images.unsplash.com/photo-1512820790803-83ca734da794" },
                new Libro { Titulo = "El Principito", Autor = "Antoine de Saint-Exupéry", Precio = 180.00m, Stock = 15, ImagenUrl = "https://images.unsplash.com/photo-1532012197267-da84d127e765" }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos.");
    }
}
// -----------------------------------------------------------

// Configure the HTTP request pipeline.
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
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();