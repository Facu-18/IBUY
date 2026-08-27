using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ItemNotaDTO : DtoBase
    {
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad solicitada debe ser mayor a cero.")]
        public decimal CantidadSolicitada { get; set; }

        public int NotaPedidoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        /// <summary>Nombre desnormalizado para mostrar sin tener que buscar el producto.</summary>
        public string ProductoNombre { get; set; } = string.Empty;

        public string UnidadMedida { get; set; } = string.Empty;
    }
}
