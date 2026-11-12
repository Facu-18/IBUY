using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    public class ItemRemito : EntityBase
    {
        [Column(TypeName = "decimal(18,2)")]
        [Range(
            typeof(decimal),
            "0.01",
            "9999999999999999.99",
            ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad debe ser mayor a cero."
        )]
        public decimal Cantidad { get; set; }

        [Required(ErrorMessage = "La unidad es obligatoria.")]
        [MaxLength(20, ErrorMessage = "La unidad no puede superar los 20 caracteres.")]
        public string Unidad { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un remito.")]
        public int RemitoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Remito Remito { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Producto Producto { get; set; } = null!;
    }
}