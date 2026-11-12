using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibreriaDiscovery.Data;

namespace LibreriaDiscovery.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Home/Index (Página principal con el banner y catálogo)
        public async Task<IActionResult> Index(string q)
        {
            var librosQuery = _context.Libros.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                librosQuery = librosQuery.Where(l => l.Titulo.Contains(q) || l.Autor.Contains(q));
            }

            var libros = await librosQuery.ToListAsync();
            return View(libros);
        }

        // GET: /Home/Catalogo (Sección dedicada al catálogo completo)
        public async Task<IActionResult> Catalogo(string q)
        {
            var librosQuery = _context.Libros.AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                librosQuery = librosQuery.Where(l => l.Titulo.Contains(q) || l.Autor.Contains(q));
            }

            var libros = await librosQuery.ToListAsync();
            return View(libros);
        }

        // GET: /Home/SobreNosotros
        public IActionResult SobreNosotros()
        {
            return View();
        }
    }
}