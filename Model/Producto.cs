using System.ComponentModel.DataAnnotations;

namespace Estharlyn_Ap1_P2.Models;

public class Producto
{
    [Key]
    public int ProductoId { get; set; }
    [Required(ErrorMessage ="El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    [Required(ErrorMessage = "El precio es obligatorio")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
