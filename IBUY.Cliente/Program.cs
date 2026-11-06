using IBUY.Cliente;
using IBUY.Cliente.Estado;
using IBUY.Servicios;
using IBUY.Servicios.ServiciosHTTP;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Estado de sesion: usuario logueado, persistido en localStorage.
builder.Services.AddSingleton<EstadoSesion>();

// El HttpClient agrega X-Usuario-Id en cada request para que la API aplique permisos.
builder.Services.AddScoped<UsuarioHeaderHandler>();
builder.Services.AddScoped(sp => new HttpClient(sp.GetRequiredService<UsuarioHeaderHandler>())
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddScoped<IHttpService, HttpService>();

// Servicios reales contra la API (usan IHttpService).
builder.Services.AddScoped<IDepositoServicio, DepositoServicioHttp>();
builder.Services.AddScoped<IProductoServicio, ProductoServicioHttp>();
builder.Services.AddScoped<INecesidadServicio, NecesidadServicioHttp>();
builder.Services.AddScoped<ICatalogoServicio, CatalogoServicioHttp>();
builder.Services.AddScoped<IEmpresaServicio, EmpresaServicioHttp>();
builder.Services.AddScoped<IUsuarioServicio, UsuarioServicioHttp>();
builder.Services.AddScoped<INotaPedidoServicio, NotaPedidoServicioHttp>();
builder.Services.AddScoped<ICotizacionServicio, CotizacionServicioHttp>();
builder.Services.AddScoped<IAuthServicio, AuthServicioHttp>();

// La sesion se restaura antes de arrancar para que el guard no redirija a /login tras un F5.
var host = builder.Build();
await host.Services.GetRequiredService<EstadoSesion>().RestaurarAsync();
await host.RunAsync();
