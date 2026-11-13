using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de la base de datos (Ejemplo usando SQLite o el proveedor que tengas configurado)
// Asegúrate de que tu cadena de conexión esté configurada en appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=libreria.db"));

// Configuración de autenticación por Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

var app = builder.Build();

// ==========================================
// SEEDING DEL ADMINISTRADOR Y BASE DE DATOS
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Crea la base de datos si no existe
        context.Database.EnsureCreated();

        var adminEmail = "admin@libreriadiscovery.com";
        if (!context.Usuarios.Any(u => u.Correo == adminEmail))
        {
            var passwordHasher = new PasswordHasher<Usuario>();
            var adminUser = new Usuario
            {
                Nombre = "Administrador",
                Correo = adminEmail,
                Rol = "Admin"
            };
            
            // Encriptamos la contraseña "Admin123*" de forma segura
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123*");
            
            context.Usuarios.Add(adminUser);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al inicializar la base de datos con el usuario Administrador.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ¡Importante! El orden de Authentication y Authorization debe ser este:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();