using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class ItemRemito : EntityBase
    {

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public int Cantidad { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un remito.")]
        public int RemitoId { get; set; }

        public Remito Remito { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        public Producto Producto { get; set; }

    }
}
