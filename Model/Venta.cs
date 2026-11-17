using System.ComponentModel.DataAnnotations;

namespace Estharlyn_Ap1_P2.Models;

public class Venta
{
    [Key]
    public int VentaId { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Today;

    [StringLength(250, ErrorMessage = "La observacion no puede exceder 250 caracteres")]
    public string? Observacion { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El total no puede ser negativo")]
    public decimal Total { get; set; }

    public ICollection<DetalleVenta> Detalles { get; set; } = new List<DetalleVenta>();
}
