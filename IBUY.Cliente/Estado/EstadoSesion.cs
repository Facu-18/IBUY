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

        /// <summary>Solo el SuperAdmin ve y gestiona las empresas de la plataforma.</summary>
        public bool EsSuperAdmin => Roles.EsSuperAdmin(Usuario?.Rol);

        /// <summary>Solo Ventas y SuperAdmin ven y gestionan usuarios.</summary>
        public bool PuedeGestionarUsuarios => PuedeGestionar;

        /// <summary>Ventas o SuperAdmin: pueden crear, editar y eliminar usuarios, depositos y productos.</summary>
        public bool PuedeGestionar => EsSuperAdmin || EsVentas;

        public bool EsVentas => Roles.EsVentas(Usuario?.Rol);

        public bool EsEncargado => Roles.EsEncargado(Usuario?.Rol);

        /// <summary>Razon social de la empresa del usuario; vacia sin sesion.</summary>
        public string Empresa => Usuario?.EmpresaRazonSocial ?? string.Empty;

        /// <summary>Id de la empresa del usuario; 0 sin sesion.</summary>
        public int EmpresaId => Usuario?.EmpresaId ?? 0;

        /// <summary>Deposito elegido en el selector global; 0 significa "Todos".</summary>
        public int DepositoSeleccionadoId { get; private set; }

        /// <summary>Se dispara cuando cambia la sesion, para que los componentes se refresquen.</summary>
        public event Action? OnCambio;

        public async Task IniciarSesionAsync(LoginRespuestaDTO usuario)
        {
            Usuario = usuario;
            DepositoSeleccionadoId = 0;
            await js.InvokeVoidAsync("localStorage.setItem", ClaveAlmacen, JsonSerializer.Serialize(usuario));
            OnCambio?.Invoke();
        }

        public async Task CerrarSesionAsync()
        {
            Usuario = null;
            DepositoSeleccionadoId = 0;
            await js.InvokeVoidAsync("localStorage.removeItem", ClaveAlmacen);
            OnCambio?.Invoke();
        }

        /// <summary>Cambia el deposito del selector global; 0 es "Todos".</summary>
        public void SeleccionarDeposito(int depositoId)
        {
            if (DepositoSeleccionadoId == depositoId)
            {
                return;
            }

            DepositoSeleccionadoId = depositoId;
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
