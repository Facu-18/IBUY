using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class ItemRemito : EntityBase
    {

        public int Cantidad { get; set; }

        public int RemitoId { get; set; }

        public Remito Remito { get; set; }

        public int ProductoId { get; set; }

        public Producto Producto { get; set; }

    }
}
