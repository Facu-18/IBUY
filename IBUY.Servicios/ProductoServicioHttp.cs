using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    /// <summary>
    /// Productos reales: consume api/producto a través de <see cref="IHttpService"/>.
    /// El endpoint devuelve las entidades, así que el recorte por empresa es del cliente.
    /// </summary>
    public class ProductoServicioHttp : IProductoServicio
    {
        private const string Base = "api/producto";
        private readonly IHttpService http;

        public ProductoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public async Task<HttpResp<List<ProductoOpcionDTO>?>> ObtenerTodosAsync(int? empresaId)
        {
            var respuesta = await http.GetAsync<List<ProductoApiDTO>>(Base);

            if (respuesta.Error)
            {
                return new HttpResp<List<ProductoOpcionDTO>?>(null, true, respuesta.Response, respuesta.Mensaje);
            }

            var productos = respuesta.Respuesta ?? [];
            if (empresaId.HasValue)
            {
                productos = [.. productos.Where(p => p.EmpresaId == empresaId.Value)];
            }

            List<ProductoOpcionDTO> opciones =
            [
                .. productos
                    .Select(p => new ProductoOpcionDTO
                    {
                        Id = p.Id,
                        Nombre = p.Nombre,
                        Categoria = p.Categoria
                    })
                    .OrderBy(p => p.Nombre)
            ];

            return new HttpResp<List<ProductoOpcionDTO>?>(opciones, false, respuesta.Response, respuesta.Mensaje);
        }

        /// <summary>Forma en que api/producto devuelve cada producto (entidad, sin navegaciones).</summary>
        private class ProductoApiDTO
        {
            public int Id { get; set; }

            public string Nombre { get; set; } = string.Empty;

            public string Categoria { get; set; } = string.Empty;

            public int EmpresaId { get; set; }
        }
    }
}
