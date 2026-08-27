using IBUY.Cliente;
using IBUY.Cliente.Estado;
using IBUY.Servicios.Interfaces;
using IBUY.Servicios.Mocks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Datos simulados: una sola instancia compartida por todos los servicios mock.
// Al recargar la pagina se vuelve a sembrar desde cero.
builder.Services.AddSingleton<BaseDatosMock>();
builder.Services.AddSingleton<IEmpresaServicio, EmpresaServicioMock>();
builder.Services.AddSingleton<IUsuarioServicio, UsuarioServicioMock>();
builder.Services.AddSingleton<IMarketplaceServicio, MarketplaceServicioMock>();

// Estado de sesion simulado (empresa, usuario y perfil activos).
builder.Services.AddSingleton<EstadoSesion>();

await builder.Build().RunAsync();
