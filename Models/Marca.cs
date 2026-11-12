using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDiscovery.Models;

[Table("Marcas")]
public class Marca
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("Nombre")]
    public required string Nombre { get; set; }
}