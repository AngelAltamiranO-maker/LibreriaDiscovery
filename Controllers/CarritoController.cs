using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using LibreriaDiscovery.Data;
using LibreriaDiscovery.Models;

namespace LibreriaDiscovery.Controllers
{
    public class CarritoController : Controller
    {
        private readonly AppDbContext _context;
        private const string SessionKey = "CarritoLibros";

        public CarritoController(AppDbContext context)
        {
            _context = context;
        }

        // Clase auxiliar para representar cada ítem en el carrito
        public class ItemCarrito
        {
            public int LibroId { get; set; } // Cambia a Guid si tu Libro.Id es Guid
            public string Titulo { get; set; } = string.Empty;
            public string Autor { get; set; } = string.Empty;
            public string ImagenUrl { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public int Cantidad { get; set; }
            public decimal Subtotal => Precio * Cantidad;
        }

        private List<ItemCarrito> ObtenerCarrito()
        {
            var json = HttpContext.Session.GetString(SessionKey);
            return string.IsNullOrEmpty(json)
                ? new List<ItemCarrito>()
                : JsonSerializer.Deserialize<List<ItemCarrito>>(json) ?? new List<ItemCarrito>();
        }

        private void GuardarCarrito(List<ItemCarrito> carrito)
        {
            HttpContext.Session.SetString(SessionKey, JsonSerializer.Serialize(carrito));
        }

        // GET: /Carrito
        public IActionResult Index()
        {
            var carrito = ObtenerCarrito();
            return View(carrito);
        }

        // POST: /Carrito/Agregar
        [HttpPost]
        public async Task<IActionResult> Agregar(int id) // Cambia a Guid si Libro.Id es Guid
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();

            var carrito = ObtenerCarrito();
            var itemExistente = carrito.FirstOrDefault(i => i.LibroId == id);

            if (itemExistente != null)
            {
                if (itemExistente.Cantidad < libro.Stock)
                {
                    itemExistente.Cantidad++;
                }
            }
            else
            {
                carrito.Add(new ItemCarrito
                {
                    LibroId = libro.Id,
                    Titulo = libro.Titulo,
                    Autor = libro.Autor,
                    ImagenUrl = libro.ImagenUrl ?? "",
                    Precio = libro.Precio,
                    Cantidad = 1
                });
            }

            GuardarCarrito(carrito);
            TempData["Mensaje"] = $"¡'{libro.Titulo}' se añadió al carrito!";

            // Regresa a la página desde donde se hizo clic (Inicio o Catálogo)
            var referer = Request.Headers["Referer"].ToString();
            return !string.IsNullOrEmpty(referer) ? Redirect(referer) : RedirectToAction("Catalogo", "Home");
        }

        // POST: /Carrito/Eliminar
        [HttpPost]
        public IActionResult Eliminar(int id) // Cambia a Guid si Libro.Id es Guid
        {
            var carrito = ObtenerCarrito();
            var item = carrito.FirstOrDefault(i => i.LibroId == id);
            if (item != null)
            {
                carrito.Remove(item);
                GuardarCarrito(carrito);
            }
            return RedirectToAction("Index");
        }

        // POST: /Carrito/Vaciar
        [HttpPost]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(SessionKey);
            return RedirectToAction("Index");
        }
    }
}