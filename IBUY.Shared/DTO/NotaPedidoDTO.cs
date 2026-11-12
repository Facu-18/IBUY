using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NotaPedidoDTO
    {
        /// <summary>Id de la nota; solo lectura, lo informa el servidor.</summary>
        public int Id { get; set; }

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

        /// <summary>Razón social de la empresa; solo lectura, lo informa el servidor.</summary>
        public string EmpresaRazonSocial { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el usuario que crea la nota de pedido.")]
        public int UsuarioId { get; set; }

        /// <summary>Nombre del usuario creador; solo lectura, lo informa el servidor.</summary>
        public string UsuarioNombre { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "El usuario aprobador no es válido.")]
        public int? AprobadoPorId { get; set; }

        /// <summary>Nombre del usuario aprobador; solo lectura, lo informa el servidor.</summary>
        public string AprobadoPorNombre { get; set; } = string.Empty;

        /// <summary>Necesidad de origen; opcional: una nota también puede crearse sola.</summary>
        [Range(1, int.MaxValue, ErrorMessage = "La necesidad asociada no es válida.")]
        public int? NecesidadId { get; set; }

        /// <summary>Cantidad de cotizaciones recibidas; solo lectura, lo informa el servidor.</summary>
        public int CantidadCotizaciones { get; set; }

        [MinLength(1, ErrorMessage = "La nota de pedido debe tener al menos un ítem.")]
        public List<ItemNotaDTO> Items { get; set; } = [];
    }
}
