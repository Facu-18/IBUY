using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    public class RecepcionRemitoDTO
    {
        [Required(ErrorMessage = "La fecha de recepción es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaRecepcion { get; set; }
    }
}
