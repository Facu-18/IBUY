using IBUY.Servicios.Mocks;
using IBUY.Shared.DTO;

namespace IBUY.Cliente.Estado
{
    public enum PerfilUsuario
    {
        Comprador,
        Proveedor
    }

    /// <summary>
    /// Estado de sesion simulado. Mientras no exista login real, toma la empresa y el
    /// usuario activos de la <see cref="BaseDatosMock"/> en lugar de tener datos propios.
    /// </summary>
    public class EstadoSesion
    {
        private readonly EmpresaDTO empresa;
        private readonly UsuarioDTO usuario;

        public EstadoSesion(BaseDatosMock bd)
        {
            empresa = bd.Empresas.First(e => e.Id == BaseDatosMock.EmpresaCompradoraId);
            usuario = bd.Usuarios.First(u => u.EmpresaId == empresa.Id);
        }

        public int EmpresaId => empresa.Id;

        public string Empresa => empresa.RazonSocial;

        public int UsuarioId => usuario.Id;

        public string Usuario => usuario.Nombre;

        public string Rol => usuario.Rol;

        public PerfilUsuario Perfil { get; private set; } = PerfilUsuario.Comprador;

        /// <summary>Se dispara cuando cambia el perfil, para que los componentes se refresquen.</summary>
        public event Action? OnCambio;

        public string Iniciales
        {
            get
            {
                var partes = Usuario.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 0)
                {
                    return "?";
                }

                return partes.Length == 1
                    ? partes[0][..1].ToUpperInvariant()
                    : $"{partes[0][0]}{partes[^1][0]}".ToUpperInvariant();
            }
        }

        public void CambiarPerfil(PerfilUsuario perfil)
        {
            if (Perfil == perfil)
            {
                return;
            }

            Perfil = perfil;
            OnCambio?.Invoke();
        }
    }
}
