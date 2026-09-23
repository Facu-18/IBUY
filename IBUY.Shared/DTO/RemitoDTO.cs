using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class RemitoDTO
    {
        [Required(ErrorMessage = "El tipo de remito es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El tipo no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de remito es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El número no puede superar los {1} caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de recepción es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRecepcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario.")]
        public int UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cotización asociada no es válida.")]
        public int? CotizacionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de origen no es válido.")]
        public int? DepositoOrigenId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de destino no es válido.")]
        public int? DepositoDestinoId { get; set; }

        [MinLength(1, ErrorMessage = "El remito debe tener al menos un ítem.")]
        public List<ItemRemitoDTO> Items { get; set; } = [];
    }
}
