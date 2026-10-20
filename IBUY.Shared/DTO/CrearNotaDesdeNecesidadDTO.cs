using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Datos que completa el usuario para generar la nota de pedido de una necesidad.
    /// El producto y la cantidad no se piden: se toman de la necesidad.
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
    }
}
