using Estharlyn_Ap1_P2.Components;
using Estharlyn_Ap1_P2.Contexto;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicios Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Conexión SQLite
var ConStr = builder.Configuration.GetConnectionString("SqlConStr");

builder.Services.AddDbContextFactory<Contexto>(options =>
    options.UseSqlite(ConStr));

// Construcción de la aplicación
var app = builder.Build();

// Configuración HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute(
    "/not-found",
    createScopeForStatusCodePages: true);

app.UseHttpsRedirection();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
