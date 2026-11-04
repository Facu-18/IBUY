namespace IBUY.Cliente.Estado
{
    /// <summary>
    /// Agrega el encabezado X-Usuario-Id a cada llamada a la API para que el servidor
    /// identifique al usuario logueado y aplique sus permisos. Sin sesion no se envia.
    /// </summary>
    public class UsuarioHeaderHandler : DelegatingHandler
    {
        /// <summary>Nombre del encabezado que lee la API (IPermisosUsuario.Encabezado).</summary>
        public const string Encabezado = "X-Usuario-Id";

        private readonly EstadoSesion sesion;

        public UsuarioHeaderHandler(EstadoSesion sesion)
        {
            this.sesion = sesion;
            InnerHandler = new HttpClientHandler();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var usuarioId = sesion.Usuario?.UsuarioId;

            if (usuarioId is int id && id > 0)
            {
                request.Headers.Remove(Encabezado);
                request.Headers.Add(Encabezado, id.ToString());
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
