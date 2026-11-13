using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // <-- ¡Vital para el PasswordHasher!
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;
using System.Security.Claims;

namespace LibreriaDiscovery.Controllers
{
    public class CuentaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher = new(); // <-- ¡Vital declarar esto aquí!

        public CuentaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Cuenta/Registro
        [HttpGet]
        public IActionResult Registro() => View();

        // POST: /Cuenta/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Registro(Usuario modelo)
        {
            // 1. Validar las reglas del modelo ([MinLength(6)])
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            // 2. Validación extra estricta de longitud
            if (string.IsNullOrEmpty(modelo.PasswordHash) || modelo.PasswordHash.Length < 6)
            {
                ModelState.AddModelError("PasswordHash", "La contraseña debe tener al menos 6 caracteres.");
                return View(modelo);
            }

            // 3. Verificar si el correo ya está registrado
            if (_context.Usuarios.Any(u => u.Correo == modelo.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo ya está registrado.");
                return View(modelo);
            }

            // 4. Encriptar la contraseña antes de guardarla
            modelo.PasswordHash = _passwordHasher.HashPassword(modelo, modelo.PasswordHash);

            modelo.Rol = "Cliente";
            modelo.FechaRegistro = DateTime.Now;

            _context.Usuarios.Add(modelo);
            _context.SaveChanges();

            TempData["Mensaje"] = "¡Registro exitoso! Ahora puedes iniciar sesión.";
            return RedirectToAction("Login");
        }

        // GET: /Cuenta/Login
        [HttpGet]
        public IActionResult Login() => View();

        // POST: /Cuenta/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string correo, string password)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == correo);

            if (usuario != null)
            {
                var resultado = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, password);

                if (resultado == PasswordVerificationResult.Success)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Nombre),
                        new Claim(ClaimTypes.Email, usuario.Correo),
                        new Claim(ClaimTypes.Role, usuario.Rol)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    if (usuario.Rol == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View();
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