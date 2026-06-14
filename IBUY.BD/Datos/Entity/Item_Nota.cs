using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class ItemNota : EntityBase
    {
        public decimal CantidadSolicitada { get; set; }

        public int NotaPedidoId { get; set; }
        public NotaPedido NotaPedido { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }
}
