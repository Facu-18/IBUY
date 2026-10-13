namespace IBUY.Shared.DTO
{
    /// <summary>Opción mínima de usuario para combos (evita exponer datos sensibles).</summary>
    public class UsuarioOpcionDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;
    }
}
