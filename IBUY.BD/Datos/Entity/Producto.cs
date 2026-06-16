using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class Producto : EntityBase
    {

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string Categoria { get; set; }

        public int EmpresaId { get; set; }


        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa Empresa { get; set; }

    }
}
