using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using static IBUY.BD.Datos.Entity.Stock;

namespace IBUY.BD.Datos.Entity
{
    public class Necesidad
    {
        public class Necesidad : EntityBase
        {

            public decimal CantidadRequerida { get; set; }

            public DateTime FechaRequerida { get; set; }

            public string Estado { get; set; }

            public int DepositoId { get; set; }

            public Deposito Deposito { get; set; }

            public int ProductoId { get; set; }

            public Producto Producto { get; set; }

        }
    }
}
