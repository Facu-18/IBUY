using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NotaPedidoDTO
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los {1} caracteres.")]
        public string Observaciones { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el usuario que crea la nota de pedido.")]
        public int UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El usuario aprobador no es válido.")]
        public int? AprobadoPorId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar la necesidad de origen.")]
        public int NecesidadId { get; set; }

        [MinLength(1, ErrorMessage = "La nota de pedido debe tener al menos un ítem.")]
        public List<ItemNotaDTO> Items { get; set; } = [];
    }
}
