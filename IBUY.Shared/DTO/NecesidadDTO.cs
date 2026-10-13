using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NecesidadDTO
    {
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
    }
}
