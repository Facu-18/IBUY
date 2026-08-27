using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NotaPedidoDTO : DtoBase
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los {1} caracteres.")]
        public string Observaciones { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        /// <summary>Razon social desnormalizada de la empresa compradora.</summary>
        public string EmpresaNombre { get; set; } = string.Empty;

        public List<ItemNotaDTO> Items { get; set; } = [];

        public bool EstaVencida => DateTime.Today > FechaVencimiento;

        public int DiasRestantes => (FechaVencimiento.Date - DateTime.Today).Days;
    }
}
