using IBUY.Shared.DTO;

namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Estado de sesion del cliente: usuario logueado, su empresa y los depositos a su cargo.
    /// Sin login, solo mantiene la empresa seleccionada en el encabezado.
    /// </summary>
    public class EstadoSesion
    {
        /// <summary>Razon social de la empresa seleccionada; vacia hasta que el encabezado cargue el listado.</summary>
        public string Empresa { get; private set; } = string.Empty;

        /// <summary>Id de la empresa seleccionada en el encabezado; 0 hasta que se cargue el listado de empresas.</summary>
        public int EmpresaId { get; private set; }

        /// <summary>Id del usuario logueado; 0 mientras no haya login.</summary>
        public int UsuarioId { get; private set; }

        /// <summary>Nombre del usuario logueado; vacio mientras no haya login.</summary>
        public string UsuarioNombre { get; private set; } = string.Empty;

        /// <summary>Rol del usuario logueado; vacio mientras no haya login.</summary>
        public string Rol { get; private set; } = string.Empty;

        /// <summary>Depositos a cargo del usuario logueado.</summary>
        public List<DepositoResumenDTO> Depositos { get; private set; } = [];

        /// <summary>Indica si hay un usuario logueado.</summary>
        public bool EstaAutenticado => UsuarioId > 0;

        /// <summary>Se dispara cuando cambia la empresa o la sesion, para que los componentes se refresquen.</summary>
        public event Action? OnCambio;

        public void CambiarEmpresa(int empresaId, string razonSocial)
        {
            // Con sesion iniciada, la empresa la define el usuario logueado.
            if (EstaAutenticado && EmpresaId != empresaId)
            {
                return;
            }

            if (EmpresaId == empresaId)
            {
                return;
            }

            EmpresaId = empresaId;
            Empresa = razonSocial;
            OnCambio?.Invoke();
        }

        /// <summary>Guarda el usuario, su empresa y los depositos a cargo que devolvio el login.</summary>
        public void IniciarSesion(LoginRespuestaDTO respuesta)
        {
            UsuarioId = respuesta.UsuarioId;
            UsuarioNombre = respuesta.Nombre;
            Rol = respuesta.Rol;
            EmpresaId = respuesta.EmpresaId;
            Empresa = respuesta.EmpresaRazonSocial;
            Depositos = respuesta.Depositos;

            OnCambio?.Invoke();
        }

        /// <summary>Cierra la sesion y conserva la empresa seleccionada para no romper el resto de las pantallas.</summary>
        public void CerrarSesion()
        {
            UsuarioId = 0;
            UsuarioNombre = string.Empty;
            Rol = string.Empty;
            Depositos = [];

            OnCambio?.Invoke();
        }
    }
}
