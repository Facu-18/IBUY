using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace IBUY.Shared.DTO
{
    public class StockDTO
    {
        [Required(ErrorMessage = "La cantidad actual es obligatoria.")]
        [Range(typeof(decimal), "0", "999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "La cantidad actual no puede ser negativa.")]
        public decimal CantidadActual { get; set; }

        [Required(ErrorMessage = "La cantidad mínima es obligatoria.")]
        [Range(typeof(decimal), "0", "999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "La cantidad mínima no puede ser negativa.")]
        public decimal CantidadMinima { get; set; }

        [Required(ErrorMessage = "El depósito es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un depósito.")]
        public int DepositoId { get; set; }

        [Required(ErrorMessage = "El producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }
    }
}
