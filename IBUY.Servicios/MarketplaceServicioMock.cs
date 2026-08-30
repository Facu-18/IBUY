using IBUY.Shared.DTO;

namespace IBUY.Servicios
{
    /// <summary>
    /// Publicaciones simuladas del marketplace. Se reinician al recargar la pagina.
    /// </summary>
    public class MarketplaceServicioMock : IMarketplaceServicio
    {
        private static readonly TimeSpan Retardo = TimeSpan.FromMilliseconds(250);

        public async Task<List<NotaPedidoDTO>> ObtenerPublicadasAsync()
        {
            await Task.Delay(Retardo);

            var hoy = DateTime.Today;

            return
            [
                new NotaPedidoDTO
                {
                    Id = 4,
                    EmpresaNombre = "Grupo Edilicio Norte SA",
                    Observaciones = "Entrega parcial aceptada.",
                    FechaVencimiento = hoy.AddDays(2),
                    Items =
                    [
                        new ItemNotaDTO { ProductoNombre = "Cal hidratada 25 kg", Cantidad = 80, UnidadMedida = "bolsa" },
                        new ItemNotaDTO { ProductoNombre = "Cano PVC 110 mm", Cantidad = 150, UnidadMedida = "unidad" }
                    ]
                },
                new NotaPedidoDTO
                {
                    Id = 3,
                    EmpresaNombre = "Constructora Andina SRL",
                    Observaciones = "Requiere flete incluido.",
                    FechaVencimiento = hoy.AddDays(8),
                    Items =
                    [
                        new ItemNotaDTO { ProductoNombre = "Arena fina", Cantidad = 25, UnidadMedida = "m3" },
                        new ItemNotaDTO { ProductoNombre = "Membrana asfaltica 4 mm", Cantidad = 40, UnidadMedida = "rollo" }
                    ]
                },
                new NotaPedidoDTO
                {
                    Id = 7,
                    EmpresaNombre = "Constructora Andina SRL",
                    Observaciones = "Compra programada para el proximo mes.",
                    FechaVencimiento = hoy.AddDays(20),
                    Items =
                    [
                        new ItemNotaDTO { ProductoNombre = "Cemento Portland 50 kg", Cantidad = 200, UnidadMedida = "bolsa" }
                    ]
                }
            ];
        }
    }
}
