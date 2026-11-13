using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;

namespace LibreriaDiscovery.Controllers
{
    [Authorize(Roles = "Admin")] // ¡Bloquea el acceso a cualquiera que no sea Administrador!
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Index
        public IActionResult Index()
        {
            var libros = _context.Libros.ToList();
            return View(libros);
        }

        // GET: /Admin/CrearLibro
        [HttpGet]
        public IActionResult CrearLibro()
        {
            return View();
        }

        // POST: /Admin/CrearLibro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearLibro(Libro libro)
        {
            if (ModelState.IsValid)
            {
                _context.Libros.Add(libro);
                _context.SaveChanges();
                TempData["Mensaje"] = "¡Libro agregado exitosamente!";
                return RedirectToAction("Index");
            }
            return View(libro);
        }

        // POST: /Admin/EliminarLibro/5
        [HttpPost]
        public IActionResult EliminarLibro(int id)
        {
            var libro = _context.Libros.Find(id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                _context.SaveChanges();
                TempData["Mensaje"] = "Libro eliminado correctamente.";
            }
            return RedirectToAction("Index");
        }
    }
}