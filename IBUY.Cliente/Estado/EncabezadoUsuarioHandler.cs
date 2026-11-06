namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Agrega a cada pedido a la API el encabezado X-Usuario-Id con el usuario logueado,
    /// que la API usa para aplicar los permisos por rol. Sin sesion no agrega nada.
    /// </summary>
    public class EncabezadoUsuarioHandler : DelegatingHandler
    {
        private const string Encabezado = "X-Usuario-Id";

        private readonly EstadoSesion estadoSesion;

        public EncabezadoUsuarioHandler(EstadoSesion estadoSesion)
        {
            this.estadoSesion = estadoSesion;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Remove(Encabezado);
            if (estadoSesion.Usuario is not null)
            {
                request.Headers.Add(Encabezado, estadoSesion.Usuario.UsuarioId.ToString());
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
