using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class EmpresaDTO
    {
        [Required(ErrorMessage = "La razón social es obligatoria.")]
        [MaxLength(100, ErrorMessage = "La razón social no puede superar los {1} caracteres.")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rubro es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El rubro no puede superar los {1} caracteres.")]
        public string Rubro { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CUIT es obligatorio.")]
        [RegularExpression(@"^\d{2}-?\d{8}-?\d$", ErrorMessage = "El CUIT debe tener 11 dígitos (formato XX-XXXXXXXX-X).")]
        [MaxLength(13, ErrorMessage = "El CUIT no puede superar los {1} caracteres.")]
        public string Cuit { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [MaxLength(100, ErrorMessage = "El email no puede superar los {1} caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de empresa es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El tipo no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;
    }
}
