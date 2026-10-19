using IBUY.Shared.DTO;
using IBUY.Servicios.ServiciosHTTP;

namespace IBUY.Servicios
{
    public class ProductoServicioHttp : IProductoServicio
    {
        private const string Base = "api/producto";
        private readonly IHttpService http;

        public ProductoServicioHttp(IHttpService http)
        {
            this.http = http;
        }

        public Task<HttpResp<List<ProductoDTO>?>> ObtenerTodosAsync(int empresaId)
        {
            return http.GetAsync<List<ProductoDTO>>($"{Base}?empresaId={empresaId}");
        }

        public Task<HttpResp<ProductoDTO?>> ObtenerPorIdAsync(int id, int empresaId)
        {
            return http.GetAsync<ProductoDTO>($"{Base}/{id}?empresaId={empresaId}");
        }

        public Task<HttpResp<int>> CrearAsync(ProductoDTO producto)
        {
            return http.PostAsync<ProductoDTO, int>(Base, producto);
        }

        public Task<HttpResp<bool>> EditarAsync(int id, ProductoDTO producto)
        {
            return http.PutAsync<ProductoDTO, bool>($"{Base}/{id}", producto);
        }
    }
}
