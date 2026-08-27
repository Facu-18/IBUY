using IBUY.Shared.DTO;

namespace IBUY.Servicios.Interfaces
{
    /// <summary>
    /// Vista de solo lectura sobre las notas de pedido: lo que ve una empresa proveedora
    /// cuando busca oportunidades para cotizar. No hereda de IServicioMock porque el
    /// marketplace no da de alta ni edita notas, solo las consulta.
    /// </summary>
    public interface IMarketplaceServicio
    {
        /// <summary>
        /// Notas publicadas, no vencidas y de otras empresas: una empresa no puede
        /// cotizar sus propias notas.
        /// </summary>
        Task<List<NotaPedidoDTO>> ObtenerPublicadasAsync(int empresaProveedoraId);

        Task<NotaPedidoDTO?> ObtenerPorIdAsync(int id);
    }
}
