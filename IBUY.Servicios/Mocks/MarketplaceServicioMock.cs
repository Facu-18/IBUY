using IBUY.Servicios.Interfaces;
using IBUY.Shared.DTO;

namespace IBUY.Servicios.Mocks
{
    public class MarketplaceServicioMock : IMarketplaceServicio
    {
        private readonly BaseDatosMock bd;

        public MarketplaceServicioMock(BaseDatosMock bd)
        {
            this.bd = bd;
        }

        public async Task<List<NotaPedidoDTO>> ObtenerPublicadasAsync(int empresaProveedoraId)
        {
            await Task.Delay(BaseDatosMock.Retardo);

            return [.. bd.NotasPedido
                .Where(nota => nota.Estado == BaseDatosMock.EstadoPublicada)
                .Where(nota => !nota.EstaVencida)
                .Where(nota => nota.EmpresaId != empresaProveedoraId)
                .OrderBy(nota => nota.FechaVencimiento)];
        }

        public async Task<NotaPedidoDTO?> ObtenerPorIdAsync(int id)
        {
            await Task.Delay(BaseDatosMock.Retardo);
            return bd.NotasPedido.FirstOrDefault(nota => nota.Id == id);
        }
    }
}
