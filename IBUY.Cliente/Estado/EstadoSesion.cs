namespace IBUY.Cliente.Estado
{
    public enum PerfilUsuario
    {
        Comprador,
        Proveedor
    }

    /// <summary>
    /// Estado de sesion simulado. Mientras no exista login real, mantiene la empresa,
    /// el usuario y el perfil activos para que el layout tenga algo que mostrar.
    /// </summary>
    public class EstadoSesion
    {
        public string Empresa { get; } = "Constructora del Sur SA";

        public string Usuario { get; } = "Sergio Algorry";

        public string Rol { get; } = "Administrador";

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
