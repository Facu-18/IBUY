using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class RemitoDTO
    {
        /// <summary>Interno (entre depósitos de la misma empresa) o Externo (llega de una empresa a la que se le compró).</summary>
        [Required(ErrorMessage = "El tipo de remito es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El tipo no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Ignorado en el POST: el servidor genera el número (R-0001).</summary>
        [MaxLength(20, ErrorMessage = "El número no puede superar los {1} caracteres.")]
        public string Numero { get; set; } = string.Empty;

        /// <summary>Ignorado en el POST: el servidor usa la fecha de hoy.</summary>
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        /// <summary>Ignorado en el POST: se completa al recibir el remito.</summary>
        [DataType(DataType.Date)]
        public DateTime? FechaRecepcion { get; set; }

        /// <summary>Ignorado en el POST: todo remito nace Emitido.</summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>Empresa a la que se le compró, escrita a mano. Obligatorio en Externo, prohibido en Interno.</summary>
        [MaxLength(100, ErrorMessage = "El origen externo no puede superar los {1} caracteres.")]
        public string? OrigenExterno { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario.")]
        public int UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cotización asociada no es válida.")]
        public int? CotizacionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de origen no es válido.")]
        public int? DepositoOrigenId { get; set; }

        /// <summary>Solo de lectura (GET): el servidor lo completa.</summary>
        public string? DepositoOrigenNombre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de destino no es válido.")]
        public int? DepositoDestinoId { get; set; }

        /// <summary>Solo de lectura (GET): el servidor lo completa.</summary>
        public string? DepositoDestinoNombre { get; set; }

        [MinLength(1, ErrorMessage = "El remito debe tener al menos un ítem.")]
        public List<ItemRemitoDTO> Items { get; set; } = [];
    }
}
