using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ProductoDTO : DtoBase
    {
        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La categoría no puede superar los {1} caracteres.")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        [MaxLength(20, ErrorMessage = "La unidad no puede superar los {1} caracteres.")]
        public string UnidadMedida { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }
    }
}
