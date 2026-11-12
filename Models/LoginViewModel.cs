using System.ComponentModel.DataAnnotations;

namespace LibreriaDiscovery.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo no válido.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = string.Empty;

        //esta línea si el controlador la requiere
        public bool Recordarme { get; set; } 
    }
}