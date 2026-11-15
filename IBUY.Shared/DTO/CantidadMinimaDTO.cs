using System.ComponentModel.DataAnnotations;

namespace IBUY.Shared.DTO
{
    /// <summary>Cuerpo de PUT api/stock/{id}/minimo.</summary>
    public class CantidadMinimaDTO
    {
        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true,
            ErrorMessage = "La cantidad mínima no puede ser negativa.")]
        public decimal CantidadMinima { get; set; }
    }
}
