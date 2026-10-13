using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static IBUY.BD.Datos.Entity.Stock;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace IBUY.BD.Datos.Entity
{

    public class Necesidad : EntityBase
    {

        [Column(TypeName = "decimal(18,2)")]
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad requerida debe ser mayor a cero.")]
        public decimal CantidadRequerida { get; set; }

        [Required(ErrorMessage = "La fecha requerida es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRequerida { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el depósito solicitante.")]
        public int DepositoSolicitanteId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito DepositoSolicitante { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el depósito destino.")]
        public int DepositoDestinoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Deposito DepositoDestino { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un producto.")]
        public int ProductoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Producto Producto { get; set; }

    }
}