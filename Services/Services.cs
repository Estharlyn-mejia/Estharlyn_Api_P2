
using Aplicada1.Core;
using Microsoft.EntityFrameworkCore;
using Estharlyn_Ap1_P2.Models;
using System.Linq.Expressions;

namespace Estharlyn_Ap1_P2.Services;

public class Services(
    IDbContextFactory<Estharlyn_Ap1_P2.Contexto.Contexto> contextFactory)
    : IServiceResult<Model, int>
{
    public Task<Result<Model?>> Buscar(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Eliminar(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Result<List<Model>>> GetList(Expression<Func<Model, bool>> criterio)
    {
        throw new NotImplementedException();
    }

    public Task<Result> Guardar(Model entidad)
    {
        throw new NotImplementedException();
    }
}
