using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NecesidadDTO
    {
        /// <summary>Id de la necesidad; 0 al crear.</summary>
        public int Id { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad requerida debe ser mayor a cero.")]
        public decimal CantidadRequerida { get; set; }

        [Required(ErrorMessage = "La fecha requerida es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRequerida { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el depósito solicitante.")]
        public int DepositoSolicitanteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el depósito destino.")]
        public int DepositoDestinoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        // Campos de solo lectura: los resuelve el servidor para que el listado y el
        // detalle no dependan de navegaciones que no se cargan.

        /// <summary>Nombre del producto de la necesidad.</summary>
        public string ProductoNombre { get; set; } = string.Empty;

        /// <summary>Categoría del producto de la necesidad.</summary>
        public string ProductoCategoria { get; set; } = string.Empty;

        /// <summary>Nombre del depósito solicitante.</summary>
        public string DepositoSolicitanteNombre { get; set; } = string.Empty;

        /// <summary>Nombre del depósito destino.</summary>
        public string DepositoDestinoNombre { get; set; } = string.Empty;

        /// <summary>Empresa dueña del depósito destino; 0 si el depósito no existe.</summary>
        public int EmpresaId { get; set; }

        /// <summary>Nota de pedido generada a partir de la necesidad; 0 si todavía no tiene.</summary>
        public int NotaPedidoId { get; set; }

        /// <summary>Stock del producto en el depósito destino.</summary>
        public decimal StockDisponible { get; set; }
    }
}
