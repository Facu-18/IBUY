using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IBUY.Shared.DTO
{
    public class DepositoDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [MaxLength(100, ErrorMessage = "El email no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        [MaxLength(30, ErrorMessage = "El rol no puede superar los {1} caracteres.")]
        public string Direccion { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }
    }
}
