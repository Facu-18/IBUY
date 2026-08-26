using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IBUY.Shared.DTO
{
    public class ProductoDTO
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(150, ErrorMessage = "Maxima longitud 150 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(150, ErrorMessage = "Maxima longitud 150 caracteres")]
    
        public string Categoria { get; set; } = string.Empty;
        [Required(ErrorMessage = "El ID de la empresa es obligatorio.")]    

        public int EmpresaId { get; set; }
    }
}
