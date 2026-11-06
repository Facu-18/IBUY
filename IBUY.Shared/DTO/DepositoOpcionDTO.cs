namespace IBUY.Shared.DTO
{
    /// <summary>Opción mínima de depósito para selectores (solo Id y Nombre).</summary>
    public class DepositoOpcionDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
