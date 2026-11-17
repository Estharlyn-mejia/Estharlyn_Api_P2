using Estharlyn_Ap1_P2.Components;
using Estharlyn_Ap1_P2.Contexto;
using Estharlyn_Ap1_P2.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Conexión SQLite: el archivo queda en la carpeta de la app
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "Estharlyn.db");

builder.Services.AddDbContextFactory<Contexto>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddScoped<ServicesVentas>();

var app = builder.Build();

// Crea las tablas (y los productos iniciales) si no existen
using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<Contexto>>();
    using var db = factory.CreateDbContext();
    db.Database.Migrate();
}

// Configuración HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();