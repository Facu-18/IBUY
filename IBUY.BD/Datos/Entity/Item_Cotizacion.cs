using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static IBUY.BD.Datos.Entity.Cotizacion;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    public class ItemCotizacion : EntityBase
    {

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad ofertada debe ser mayor a cero.")]
        public decimal CantidadOfertada { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "El precio unitario no puede ser negativo.")]
        public decimal PrecioUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "El precio total no puede ser negativo.")]
        public decimal PrecioTotal { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una cotización.")]
        public int CotizacionId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Cotizacion Cotizacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un ítem de la nota de pedido.")]
        public int ItemNotaId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public ItemNota ItemNota { get; set; }

    }
}
