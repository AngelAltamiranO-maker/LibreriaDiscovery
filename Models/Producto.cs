using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDiscovery.Models;

[Table("Productos")]
public class Producto
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [Column("Descripcion")]
    public required string Nombre { get; set; }

    [Column("Precio")]
    public decimal Precio { get; set; }

    [Column("Existencia")]
    public int Stock { get; set; }

    [Column("IdMarca")]
    public int MarcaId { get; set; }

    [ForeignKey("MarcaId")]
    public virtual Marca? Marca { get; set; } // <--- Añadimos el signo '?' aquí
}