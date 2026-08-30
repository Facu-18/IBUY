using IBUY.Shared.DTO;

namespace IBUY.Servicios
{
    /// <summary>
    /// Catalogo simulado, en memoria. Se reinicia al recargar la pagina.
    /// </summary>
    public class ProductoServicioMock : IProductoServicio
    {
        private static readonly TimeSpan Retardo = TimeSpan.FromMilliseconds(250);

        private readonly List<ProductoDTO> productos =
        [
            new ProductoDTO { Nombre = "Cemento Portland 50 kg", Descripcion = "Bolsa de 50 kg, uso general", Categoria = "Aridos y cementos", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Hierro nervurado del 8", Descripcion = "Barra de 12 m", Categoria = "Hierros", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Ladrillo hueco 12x18x33", Descripcion = "Ceramico portante", Categoria = "Mamposteria", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Arena fina", Descripcion = "A granel, por metro cubico", Categoria = "Aridos y cementos", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Membrana asfaltica 4 mm", Descripcion = "Rollo de 10 m con aluminio", Categoria = "Impermeabilizantes", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Cal hidratada 25 kg", Descripcion = "Bolsa de 25 kg", Categoria = "Aridos y cementos", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Cano PVC 110 mm", Descripcion = "Tira de 4 m para desague", Categoria = "Sanitarios", EmpresaId = 1 },
            new ProductoDTO { Nombre = "Pintura latex interior 20 L", Descripcion = "Balde de 20 litros, blanco mate", Categoria = "Pinturas", EmpresaId = 1 }
        ];

        public async Task<List<ProductoDTO>> ObtenerTodosAsync()
        {
            await Task.Delay(Retardo);
            return [.. productos];
        }
    }
}
