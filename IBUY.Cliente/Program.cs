using IBUY.Cliente;
using IBUY.Cliente.Estado;
using IBUY.Servicios;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Estado de sesion simulado (empresa, usuario y perfil activos).
builder.Services.AddSingleton<EstadoSesion>();

// Servicios simulados: datos en memoria, se reinician al recargar la pagina.
builder.Services.AddSingleton<IProductoServicio, ProductoServicioMock>();
builder.Services.AddSingleton<IMarketplaceServicio, MarketplaceServicioMock>();

await builder.Build().RunAsync();
