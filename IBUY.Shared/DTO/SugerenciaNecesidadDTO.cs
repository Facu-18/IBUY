namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Producto con stock crítico dentro de un depósito, con la necesidad que conviene cargar.
    /// Solo informa: el usuario decide si la crea desde el formulario.
    /// </summary>
    public class SugerenciaNecesidadDTO
    {
        public int ProductoId { get; set; }

        public string ProductoNombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        /// <summary>Depósito donde falta el producto.</summary>
        public int DepositoDestinoId { get; set; }

        public string DepositoDestinoNombre { get; set; } = string.Empty;

        /// <summary>Depósito propuesto para pedir el producto; 0 si la empresa no tiene otro depósito.</summary>
        public int DepositoSolicitanteId { get; set; }

        public string DepositoSolicitanteNombre { get; set; } = string.Empty;

        /// <summary>Stock del producto en el depósito destino.</summary>
        public decimal CantidadActual { get; set; }

        /// <summary>Cantidad propuesta para la necesidad.</summary>
        public decimal CantidadSugerida { get; set; }

        /// <summary>True si ya existe una necesidad pendiente para ese producto y esos depósitos.</summary>
        public bool YaTienePendiente { get; set; }
    }
}
