using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class DepositoDTO
    {
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
