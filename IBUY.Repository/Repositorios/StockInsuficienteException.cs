namespace IBUY.Repository.Repositorios
{
    /// <summary>Se lanza cuando un movimiento de salida/transferencia no tiene stock suficiente en el origen.</summary>
    public class StockInsuficienteException : Exception
    {
        public int DepositoId { get; }

        public int ProductoId { get; }

        public StockInsuficienteException(int depositoId, int productoId)
            : base($"No hay stock suficiente del producto {productoId} en el depósito {depositoId}.")
        {
            DepositoId = depositoId;
            ProductoId = productoId;
        }
    }
}
