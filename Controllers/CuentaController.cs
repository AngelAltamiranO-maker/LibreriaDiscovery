using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;
using System.Security.Claims;

namespace LibreriaDiscovery.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Cuenta/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Cuenta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string password)
        {
            // Buscar al usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo && u.PasswordHash == password);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View();
            }

            // Crear las credenciales de sesión (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol) // Aquí guardamos si es "Admin" o "Cliente"
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Redirigir según su rol
            if (usuario.Rol == "Admin")
            {
                return RedirectToAction("Index", "Admin"); // Pronto crearemos este controlador
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Cuenta/Registro
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Cuenta/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario modelo)
        {
            // Verificar si el correo ya está registrado
            if (_context.Usuarios.Any(u => u.Correo == modelo.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado.");
                return View(modelo);
            }

            modelo.Rol = "Cliente"; // Por defecto, cualquier registro web es cliente
            modelo.FechaRegistro = DateTime.Now;

            _context.Usuarios.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // POST: /Cuenta/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}