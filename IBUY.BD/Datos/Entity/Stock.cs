using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    public class Stock : EntityBase
    {
        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad actual no puede ser negativa.")]
        public decimal CantidadActual { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un depósito.")]
        public int DepositoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito Deposito { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Producto Producto { get; set; }
    }
}
