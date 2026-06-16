using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
    public class ItemNota : EntityBase
    {
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadSolicitada { get; set; }

        public int NotaPedidoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public NotaPedido NotaPedido { get; set; }

        public int ProductoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Producto Producto { get; set; }
    }
}
