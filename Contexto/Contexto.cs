
using Microsoft.EntityFrameworkCore;
using Estharlyn_Ap1_P2.Models;

namespace Estharlyn_Ap1_P2.Contexto;

public class Contexto(DbContextOptions<Contexto> options)
    : DbContext(options)
{
    public DbSet<Model> Modelos { get; set; }
}
