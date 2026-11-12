using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ItemRemitoDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        /// <summary>Solo de lectura (GET): el servidor lo completa.</summary>
        public string? ProductoNombre { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "La unidad es obligatoria.")]
        [MaxLength(20, ErrorMessage = "La unidad no puede superar los {1} caracteres.")]
        public string Unidad { get; set; } = string.Empty;
    }
}
