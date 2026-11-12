using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;

namespace LibreriaDiscovery.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;

        public ProductosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Productos/Index (Catálogo completo)
        public async Task<IActionResult> Index(string q, int? marca)
        {
            var query = _context.Productos.Include(p => p.Marca).AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(p => p.Nombre.Contains(q));
            }

            if (marca.HasValue && marca.Value > 0)
            {
                query = query.Where(p => p.MarcaId == marca.Value);
            }

            ViewBag.Marcas = await _context.Marcas.ToListAsync();
            return View(await query.ToListAsync());
        }

        // GET: /Productos/Details/id (Ver detalle de un libro)
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null) return NotFound();

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (producto == null) return NotFound();

            return View(producto);
        }

        // Aquí agregaremos las acciones de Create, Edit y Delete en el siguiente paso.
    }
}