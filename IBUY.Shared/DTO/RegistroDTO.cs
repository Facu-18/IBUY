using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Alta de una empresa nueva junto con su primer usuario (registro público).
    /// Es plano a propósito: el validador de formularios de Blazor no valida objetos anidados.
    /// Las reglas de la empresa son las mismas que las de <see cref="EmpresaDTO"/>.
    /// </summary>
    public class RegistroDTO
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

        [Required(ErrorMessage = "El email de la empresa es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email de la empresa no tiene un formato válido.")]
        [MaxLength(100, ErrorMessage = "El email de la empresa no puede superar los {1} caracteres.")]
        public string EmailEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [MaxLength(100, ErrorMessage = "El email no puede superar los {1} caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;
    }
}
