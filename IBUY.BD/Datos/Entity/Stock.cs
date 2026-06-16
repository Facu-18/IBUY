using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    public class Stock : EntityBase
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadActual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadMinima { get; set; }

        public int DepositoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito Deposito { get; set; }

        public int ProductoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Producto Producto { get; set; }
    }
}
