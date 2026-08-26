namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Identificador comun a todos los DTO de lectura. Los servicios lo usan
    /// para relacionar los datos entre si.
    /// </summary>
    public abstract class DtoBase
    {
        public int Id { get; set; }
    }
}
