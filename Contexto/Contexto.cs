
using Microsoft.EntityFrameworkCore;
using Estharlyn_Ap1_P2.Models;

namespace Estharlyn_Ap1_P2.Contexto;

public class Contexto(DbContextOptions<Contexto> options)
    : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Producto>()
            .ToTable("Productos")
            .Property(producto => producto.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Venta>()
            .Property(venta => venta.Total)
            .HasPrecision(12, 2);

        modelBuilder.Entity<DetalleVenta>()
            .Property(detalle => detalle.Precio)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Venta>()
            .HasMany(venta => venta.Detalles)
            .WithOne(detalle => detalle.Venta)
            .HasForeignKey(detalle => detalle.VentaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Producto>()
            .HasMany(producto => producto.Detalles)
            .WithOne(detalle => detalle.Producto)
            .HasForeignKey(detalle => detalle.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Producto>().HasData(
            new Producto { ProductoId = 1, Nombre = "Cafe Latte", Precio = 75m, Stock = 10 },
            new Producto { ProductoId = 2, Nombre = "Cafe Mocha", Precio = 85m, Stock = 10 },
            new Producto { ProductoId = 3, Nombre = "Caramelo Macchiato", Precio = 90m, Stock = 10 },
            new Producto { ProductoId = 4, Nombre = "Cafe Vainilla", Precio = 80m, Stock = 10 },
            new Producto { ProductoId = 5, Nombre = "Cafe Espresso", Precio = 65m, Stock = 10 });
    }
}
