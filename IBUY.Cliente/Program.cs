using IBUY.Cliente;
using IBUY.Cliente.Estado;
using IBUY.Servicios;
using IBUY.Servicios.ServiciosHTTP;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Estado de sesion: empresa seleccionada en el encabezado (todavia no hay login).
builder.Services.AddSingleton<EstadoSesion>();

builder.Services.AddScoped<IHttpService, HttpService>();

// Servicios reales contra la API (usan IHttpService).
builder.Services.AddScoped<IDepositoServicio, DepositoServicioHttp>();
builder.Services.AddScoped<IProductoServicio, ProductoServicioHttp>();
builder.Services.AddScoped<ICatalogoServicio, CatalogoServicioHttp>();

await builder.Build().RunAsync();
