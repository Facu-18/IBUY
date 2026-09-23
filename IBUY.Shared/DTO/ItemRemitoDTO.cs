using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ItemRemitoDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public decimal Cantidad { get; set; }
    }
}
