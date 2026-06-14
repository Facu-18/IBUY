using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class Empresa : EntityBase
    {

        public string RazonSocial { get; set; } =
        string.Empty;

        public string Rubro { get; set; } =
        string.Empty;

        public string Cuit { get; set; } =
        string.Empty;

        public string Email { get; set; } =
        string.Empty;

        public string Tipo { get; set; } =
        string.Empty;
        public bool Activo { get; set; } = true;

    }

}
