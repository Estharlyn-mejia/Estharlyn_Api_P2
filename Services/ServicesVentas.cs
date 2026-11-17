using Aplicada1.Core;
using Estharlyn_Ap1_P2.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Estharlyn_Ap1_P2.Services;

public class Services(IDbContextFactory<Estharlyn_Ap1_P2.Contexto.Contexto> contextFactory)
    : IServiceResult<Venta, int>
{
    public async Task<Result> Guardar(Venta venta)
    {
        if (venta.Detalles is null || venta.Detalles.Count == 0)
            return Fallo("VENTA_SIN_DETALLES", "Debe agregar al menos un detalle a la venta.");

        if (venta.Detalles.Any(detalle => detalle.ProductoId <= 0 || detalle.Cantidad <= 0))
            return Fallo("DETALLE_INVALIDO", "Cada detalle debe tener un producto y una cantidad mayor a cero.");

        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();

        Venta? original = null;
        if (venta.VentaId > 0)
        {
            original = await contexto.Ventas
                .Include(registro => registro.Detalles)
                .SingleOrDefaultAsync(registro => registro.VentaId == venta.VentaId);

            if (original is null)
                return Fallo("VENTA_NO_ENCONTRADA", "No se encontro la venta que se intenta modificar.");
        }

        var productoIds = venta.Detalles.Select(detalle => detalle.ProductoId)
            .Concat(original?.Detalles.Select(detalle => detalle.ProductoId) ?? Enumerable.Empty<int>())
            .Distinct()
            .ToList();

        var productos = await contexto.Productos
            .Where(producto => productoIds.Contains(producto.ProductoId))
            .ToDictionaryAsync(producto => producto.ProductoId);

        if (productos.Count != productoIds.Count)
            return Fallo("PRODUCTO_NO_ENCONTRADO", "Uno de los productos seleccionados no existe.");

        if (original is not null)
        {
            foreach (var detalleAnterior in original.Detalles)
                productos[detalleAnterior.ProductoId].Stock += detalleAnterior.Cantidad;
        }

        foreach (var grupo in venta.Detalles.GroupBy(detalle => detalle.ProductoId))
        {
            var cantidadSolicitada = grupo.Sum(detalle => (long)detalle.Cantidad);
            var producto = productos[grupo.Key];

            if (cantidadSolicitada > producto.Stock)
                return Fallo(
                    "STOCK_INSUFICIENTE",
                    $"Stock insuficiente para {producto.Nombre}. Disponible: {producto.Stock}; solicitado: {cantidadSolicitada}.");
        }

        foreach (var grupo in venta.Detalles.GroupBy(detalle => detalle.ProductoId))
        {
            var cantidadSolicitada = (int)grupo.Sum(detalle => (long)detalle.Cantidad);
            productos[grupo.Key].Stock -= cantidadSolicitada;
        }

        var nuevosDetalles = venta.Detalles.Select(detalle => new DetalleVenta
        {
            ProductoId = detalle.ProductoId,
            Cantidad = detalle.Cantidad,
            Precio = productos[detalle.ProductoId].Precio
        }).ToList();

        var total = nuevosDetalles.Sum(detalle => detalle.Cantidad * detalle.Precio);

        if (original is null)
        {
            original = new Venta
            {
                Fecha = venta.Fecha,
                Observacion = venta.Observacion?.Trim(),
                Total = total,
                Detalles = nuevosDetalles
            };
            contexto.Ventas.Add(original);
        }
        else
        {
            original.Fecha = venta.Fecha;
            original.Observacion = venta.Observacion?.Trim();
            original.Total = total;
            original.Detalles.Clear();
            foreach (var detalle in nuevosDetalles)
                original.Detalles.Add(detalle);
        }

        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();
        venta.Total = total;
        venta.VentaId = original.VentaId;

        return Result.Success(true);
    }

    public async Task<Result<Venta?>> Buscar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(venta => venta.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .AsNoTracking()
            .FirstOrDefaultAsync(venta => venta.VentaId == id);
    }

    public async Task<Result<List<Venta>>> GetList(Expression<Func<Venta, bool>> criterio)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Ventas
            .Include(venta => venta.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .Where(criterio)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Result> Eliminar(int id)
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        await using var transaccion = await contexto.Database.BeginTransactionAsync();

        var venta = await contexto.Ventas
            .Include(registro => registro.Detalles)
                .ThenInclude(detalle => detalle.Producto)
            .FirstOrDefaultAsync(registro => registro.VentaId == id);

        if (venta is null)
            return Fallo("VENTA_NO_ENCONTRADA", "Venta no encontrada.");

        foreach (var detalle in venta.Detalles)
            detalle.Producto.Stock += detalle.Cantidad;

        contexto.Ventas.Remove(venta);
        await contexto.SaveChangesAsync();
        await transaccion.CommitAsync();

        return Result.Success(true);
    }

    public async Task<List<Producto>> ListarProductos()
    {
        await using var contexto = await contextFactory.CreateDbContextAsync();
        return await contexto.Productos
            .AsNoTracking()
            .OrderBy(producto => producto.Nombre)
            .ToListAsync();
    }

    private static Result Fallo(string codigo, string descripcion) =>
        Result.Failure(new Error(codigo, descripcion));
}
