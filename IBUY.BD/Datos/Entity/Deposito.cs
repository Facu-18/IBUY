using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class Deposito : EntityBase
    {

        public string Nombre { get; set; }

        public string Tipo { get; set; }

        public string Direccion { get; set; }

        public bool Activo { get; set; }

        public int EmpresaId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa Empresa { get; set; }

    }
}
