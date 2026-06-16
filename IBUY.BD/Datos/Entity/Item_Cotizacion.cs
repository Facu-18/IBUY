using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using static IBUY.BD.Datos.Entity.Cotizacion;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{
        public class ItemCotizacion : EntityBase
        {

            [Column(TypeName = "decimal(18,2)")]
            public decimal CantidadOfertada { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal PrecioUnitario { get; set; }

            [Column(TypeName = "decimal(18,2)")]
            public decimal PrecioTotal { get; set; }

            public int CotizacionId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public Cotizacion Cotizacion { get; set; }

            public int ItemNotaId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public ItemNota ItemNota { get; set; }

        }
}
