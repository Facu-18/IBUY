using Proyecto2026.BD.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace IBUY.BD.Datos.Entity
{
    public class Cotizacion : EntityBase
    {

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(20, ErrorMessage = "El estado no puede superar los {1} caracteres.")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El plazo de entrega es obligatorio.")]
        [MaxLength(50, ErrorMessage = "El plazo de entrega no puede superar los {1} caracteres.")]
        public string PlazoEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de envío es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaEnvio { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una nota de pedido.")]
        public int NotaPedidoId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public NotaPedido NotaPedido { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una empresa proveedora.")]
        public int EmpresaProveedoraId { get; set; }

        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Empresa EmpresaProveedora { get; set; }

    }
}
