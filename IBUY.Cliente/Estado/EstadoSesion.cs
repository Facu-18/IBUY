using System.Text.Json;
using IBUY.Shared.DTO;
using Microsoft.JSInterop;

namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Estado de sesion del cliente: usuario logueado (con su empresa y depositos a cargo),
    /// persistido en localStorage para sobrevivir a una recarga. La empresa activa es la del usuario.
    /// </summary>
    public class EstadoSesion
    {
        private const string ClaveAlmacen = "ibuy.sesion";

        private readonly IJSRuntime js;

        public EstadoSesion(IJSRuntime js)
        {
            this.js = js;
        }

        /// <summary>Usuario logueado; null si no hay sesion.</summary>
        public LoginRespuestaDTO? Usuario { get; private set; }

        public bool EstaLogueado => Usuario is not null;

        /// <summary>Razon social de la empresa del usuario; vacia sin sesion.</summary>
        public string Empresa => Usuario?.EmpresaRazonSocial ?? string.Empty;

        /// <summary>Id de la empresa del usuario; 0 sin sesion.</summary>
        public int EmpresaId => Usuario?.EmpresaId ?? 0;

        /// <summary>Se dispara cuando cambia la sesion, para que los componentes se refresquen.</summary>
        public event Action? OnCambio;

        public async Task IniciarSesionAsync(LoginRespuestaDTO usuario)
        {
            Usuario = usuario;
            await js.InvokeVoidAsync("localStorage.setItem", ClaveAlmacen, JsonSerializer.Serialize(usuario));
            OnCambio?.Invoke();
        }

        public async Task CerrarSesionAsync()
        {
            Usuario = null;
            await js.InvokeVoidAsync("localStorage.removeItem", ClaveAlmacen);
            OnCambio?.Invoke();
        }

        /// <summary>Recupera la sesion guardada; un valor ausente o corrupto se descarta.</summary>
        public async Task RestaurarAsync()
        {
            var json = await js.InvokeAsync<string?>("localStorage.getItem", ClaveAlmacen);
            if (string.IsNullOrWhiteSpace(json))
            {
                return;
            }

            try
            {
                Usuario = JsonSerializer.Deserialize<LoginRespuestaDTO>(json);
            }
            catch (JsonException)
            {
                Usuario = null;
            }

            if (Usuario is null)
            {
                await js.InvokeVoidAsync("localStorage.removeItem", ClaveAlmacen);
            }
        }
    }
}
