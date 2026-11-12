using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;

namespace LibreriaDiscovery.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        // GET: /Account/Register
        public IActionResult Register() => View();

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo))
            {
                ModelState.AddModelError("Email", "Este correo electrónico ya está registrado.");
                return View(model);
            }

            var usuario = new Usuario
            {
                Nombre = model.Nombre,
                Correo = model.Correo,
                Rol = "Cliente" // Rol por defecto
            };

            // Encriptado de contraseña seguro
            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, model.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Iniciar sesión automáticamente tras registrarse
            await IniciarSesion(usuario, isPersistent: false);

            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Login
        public IActionResult Login() => View();

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == model.Correo);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            // Verificar la contraseña encriptada
            var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, model.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(model);
            }

            await IniciarSesion(usuario, model.Recordarme);

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task IniciarSesion(Usuario usuario, bool isPersistent)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = isPersistent };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
    }
}