using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.BD.Datos.Entity
{
    public class Remito : EntityBase
    {

        public string Tipo { get; set; }

        public string Numero { get; set; }

        public DateTime FechaEmision { get; set; }

        public DateTime FechaRecepcion { get; set; }

        public int EmpresaId { get; set; }

        public Empresa Empresa { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }

        public int? CotizacionId { get; set; }

        public Cotizacion Cotizacion { get; set; }

        public int? DepositoOrigenId { get; set; }

        public Deposito DepositoOrigen { get; set; }

        public int? DepositoDestinoId { get; set; }

        public Deposito DepositoDestino { get; set; }

    }



}
