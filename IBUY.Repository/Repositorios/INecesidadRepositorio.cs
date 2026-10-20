using IBUY.BD.Datos.Entity;
using IBUY.Shared.DTO;

namespace IBUY.Repository.Repositorios
{
    public interface INecesidadRepositorio : IRepositorio<Necesidad>
    {
        /// <summary>Cantidad actual de un producto en un depósito (0 si no hay fila de stock).</summary>
        Task<decimal> ObtenerStockDisponible(int depositoId, int productoId);

        /// <summary>
        /// True si ya hay una necesidad pendiente con el mismo producto y los mismos depósitos.
        /// <paramref name="excluirId"/> permite ignorar la necesidad que se está editando.
        /// </summary>
        Task<bool> ExistePendienteDuplicada(int depositoSolicitanteId, int depositoDestinoId, int productoId, int? excluirId = null);

        /// <summary>Necesidades de los depósitos de una empresa, con filtros opcionales de estado y depósito.</summary>
        Task<List<Necesidad>> ObtenerDeEmpresa(int empresaId, string? estado, int? depositoId);

        /// <summary>Depósitos de una empresa (para resolver nombres y validar pertenencia).</summary>
        Task<List<Deposito>> ObtenerDepositosDeEmpresa(int empresaId);

        /// <summary>Filas de stock de un depósito, para detectar productos con stock crítico.</summary>
        Task<List<Stock>> ObtenerStockDeDeposito(int depositoId);

        /// <summary>Notas de pedido generadas a partir de necesidades, indexadas por necesidad.</summary>
        Task<Dictionary<int, int>> ObtenerNotasPorNecesidad();

        /// <summary>
        /// Crea la nota de pedido de una necesidad en una sola transacción: un ítem con el
        /// producto y la cantidad de la necesidad, y el estado de la necesidad a "En nota".
        /// Devuelve la nota creada.
        /// </summary>
        Task<NotaPedido> GenerarNotaPedido(Necesidad necesidad, NotaPedido notaPedido);
    }
}
