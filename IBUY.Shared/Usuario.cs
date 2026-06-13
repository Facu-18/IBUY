using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.Shared
{
    public class Usuario : EntityBase
    {

        public string Nombre { get; set; } =
        string.Empty;

        public string Email { get; set; } =
        string.Empty;

        public string Contrasena { get; set; } =
        string.Empty;

        public string Rol { get; set; } =
        string.Empty;

        public bool Estado { get; set; } = true;

        public int EmpresaId { get; set; } 

        public required Empresa Empresa { get; set; }  

    }
}
