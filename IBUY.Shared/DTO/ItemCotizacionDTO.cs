using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ItemCotizacionDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un ítem de la nota de pedido.")]
        public int ItemNotaId { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad ofertada debe ser mayor a cero.")]
        public decimal CantidadOfertada { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "El precio total no puede ser negativo.")]
        public decimal PrecioTotal { get; set; }
    }
}
