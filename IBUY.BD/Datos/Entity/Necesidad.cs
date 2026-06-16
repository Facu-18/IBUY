using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using static IBUY.BD.Datos.Entity.Stock;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    
        public class Necesidad : EntityBase
        {

            [Column(TypeName = "decimal(18,2)")]
            public decimal CantidadRequerida { get; set; }

            public DateTime FechaRequerida { get; set; }

            public string Estado { get; set; }

            public int DepositoId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public Deposito Deposito { get; set; }

            public int ProductoId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public Producto Producto { get; set; }

        }
    }

