using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class NotaPedido : EntityBase
    {

        public string Estado { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public string Observaciones { get; set; }

        public int EmpresaId { get; set; }


        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa Empresa { get; set; }

        public int UsuarioId { get; set; } // creador 

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Usuario Usuario { get; set; }

        public int? AprobadoPorId { get; set; } // aprobador

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Usuario AprobadoPor { get; set; }

        public int? NecesidadId { get; set; }


        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Necesidad Necesidad { get; set; }

    }
}
