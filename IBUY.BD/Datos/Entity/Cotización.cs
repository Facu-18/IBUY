using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class Cotización
    {
        public class Cotizacion : EntityBase
        {

            public string Estado { get; set; }

            public string PlazoEntrega { get; set; }

            public DateTime FechaEnvio { get; set; }

            public int NotaPedidoId { get; set; }

            public NotaPedido NotaPedido { get; set; }

            public int EmpresaProveedoraId { get; set; }

            public Empresa EmpresaProveedora { get; set; }

        }
    }
}
