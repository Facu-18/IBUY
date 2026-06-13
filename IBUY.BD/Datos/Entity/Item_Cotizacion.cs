using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using static IBUY.BD.Datos.Entity.Cotización;

namespace IBUY.BD.Datos.Entity
{
    internal class Item_Cotizacion
    {
        public class ItemCotizacion : EntityBase
        {

            public decimal CantidadOfertada { get; set; }

            public decimal PrecioUnitario { get; set; }

            public decimal PrecioTotal { get; set; }

            public int CotizacionId { get; set; }

            public Cotizacion Cotizacion { get; set; }

            public int ItemNotaId { get; set; }

            public ItemNota ItemNota { get; set; }

        }


    }
}
