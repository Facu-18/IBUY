namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Estado de sesion del cliente. Mientras no exista login, solo mantiene la empresa
    /// seleccionada en el encabezado.
    /// </summary>
    public class EstadoSesion
    {
        /// <summary>Razon social de la empresa seleccionada; vacia hasta que el encabezado cargue el listado.</summary>
        public string Empresa { get; private set; } = string.Empty;

        /// <summary>Id de la empresa seleccionada en el encabezado; 0 hasta que se cargue el listado de empresas.</summary>
        public int EmpresaId { get; private set; }

        /// <summary>Se dispara cuando cambia la empresa, para que los componentes se refresquen.</summary>
        public event Action? OnCambio;

        public void CambiarEmpresa(int empresaId, string razonSocial)
        {
            if (EmpresaId == empresaId)
            {
                return;
            }

            EmpresaId = empresaId;
            Empresa = razonSocial;
            OnCambio?.Invoke();
        }
    }
}
