namespace IBUY.Shared.DTO
{
    /// <summary>Resumen de depósito usado en la respuesta de login (depósitos a cargo del usuario).</summary>
    public class DepositoResumenDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
