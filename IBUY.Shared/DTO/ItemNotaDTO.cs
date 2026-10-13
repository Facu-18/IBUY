using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ItemNotaDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad solicitada debe ser mayor a cero.")]
        public decimal CantidadSolicitada { get; set; }
    }
}
