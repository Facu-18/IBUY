using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class Remito : EntityBase
    {

        [Required(ErrorMessage = "El tipo de remito es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El tipo no puede superar los {1} caracteres.")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de remito es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El número no puede superar los {1} caracteres.")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de recepción es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRecepcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa Empresa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un usuario.")]
        public int UsuarioId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Usuario Usuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cotización asociada no es válida.")]
        public int? CotizacionId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Cotizacion Cotizacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de origen no es válido.")]
        public int? DepositoOrigenId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito DepositoOrigen { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El depósito de destino no es válido.")]
        public int? DepositoDestinoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito DepositoDestino { get; set; }

    }



}
