using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class NotaPedido : EntityBase
    {

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de emisión es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaEmision { get; set; }

        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(500, ErrorMessage = "Las observaciones no pueden superar los {1} caracteres.")]
        public string Observaciones { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa.")]
        public int EmpresaId { get; set; }


        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa Empresa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar el usuario que crea la nota de pedido.")]
        public int UsuarioId { get; set; } // creador

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Usuario Usuario { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El usuario aprobador no es válido.")]
        public int? AprobadoPorId { get; set; } // aprobador

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Usuario AprobadoPor { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La necesidad asociada no es válida.")]
        public int? NecesidadId { get; set; }


        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Necesidad Necesidad { get; set; }

    }
}
