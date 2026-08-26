namespace IBUY.Server.Controllers
{
    public class ProductoDTO
    {
        internal Producto Nombre;
        internal Producto Categoria;

        public Producto Descripcion { get; internal set; }
        public Producto EmpresaId { get; internal set; }
    }
}