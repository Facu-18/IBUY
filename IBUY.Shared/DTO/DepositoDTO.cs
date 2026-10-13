using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class DepositoDTO
    {
        /// <summary>Solo lectura: lo informa el servidor en los GET.</summary>
        public int Id { get; set; }

        /// <summary>Solo lectura: razón social de la empresa a la que pertenece.</summary>
        public string EmpresaRazonSocial { get; set; } = string.Empty;

        /// <summary>Solo lectura: nombre del usuario responsable, si está asignado.</summary>
        public string UsuarioResponsableNombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del depósito es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de depósito es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El tipo no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [MaxLength(200, ErrorMessage = "La dirección no puede superar los {1} caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El usuario responsable no es válido.")]
        public int? UsuarioResponsableId { get; set; }
    }
}
