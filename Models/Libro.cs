using System.ComponentModel.DataAnnotations;

namespace LibreriaDiscovery.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El autor no puede superar los 100 caracteres.")]
        public string Autor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 10000.00, ErrorMessage = "El precio debe estar entre 0.01 y 10,000.")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, 10000, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        [Display(Name = "URL de la Imagen")]
        public string ImagenUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        public string Descripcion { get; set; } = string.Empty;
    }
}