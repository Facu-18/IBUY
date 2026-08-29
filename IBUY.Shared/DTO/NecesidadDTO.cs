using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class NecesidadDTO
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(1500, ErrorMessage = "Maxima longitud 1500 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(1500, ErrorMessage = "Maxima longitud 1500 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(1500, ErrorMessage = "Maxima longitud 1500 caracteres")]
        public string Categoria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El ID de la empresa es obligatorio.")]
        public int EmpresaId { get; set; }
        public string Estado { get; set; }
    }
}
