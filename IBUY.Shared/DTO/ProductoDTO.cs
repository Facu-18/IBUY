using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class ProductoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La descripcion no puede superar los {1} caracteres.")]
        public string? Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoria es obligatoria.")]
        [MaxLength(50, ErrorMessage = "La categoria no puede superar los {1} caracteres.")]
        public string Categoria { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }
    }
}
