using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Datos que completa el usuario para generar la nota de pedido de una necesidad.
    /// El producto se toma de la necesidad. La cantidad, si no se informa, es el faltante:
    /// lo requerido menos el stock del depósito que abastece.
    /// </summary>
    public class CrearNotaDesdeNecesidadDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el usuario que crea la nota de pedido.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los {1} caracteres.")]
        public string Observaciones { get; set; } = string.Empty;

        /// <summary>Cantidad a pedir; vacía = el faltante calculado por el servidor.</summary>
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad a pedir debe ser mayor a cero.")]
        public decimal? CantidadSolicitada { get; set; }
    }
}
