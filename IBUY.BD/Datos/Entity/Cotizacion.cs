using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
        public class Cotizacion : EntityBase
        {

            public string Estado { get; set; }

            public string PlazoEntrega { get; set; }

            public DateTime FechaEnvio { get; set; }

            public int NotaPedidoId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public NotaPedido NotaPedido { get; set; }

            public int EmpresaProveedoraId { get; set; }

            [DeleteBehavior(DeleteBehavior.NoAction)]
            public Empresa EmpresaProveedora { get; set; }

    }
}
