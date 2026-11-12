using System.ComponentModel.DataAnnotations;

namespace LibreriaDiscovery.Models
{
    public class Usuario
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Rol: "Admin" o "Cliente"
        public string Rol { get; set; } = "Cliente";

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}