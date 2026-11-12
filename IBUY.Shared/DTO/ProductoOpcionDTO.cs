namespace IBUY.Shared.DTO
{
    /// <summary>Opción mínima de producto para combos (id, nombre y categoría).</summary>
    public class ProductoOpcionDTO
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;
    }
}
