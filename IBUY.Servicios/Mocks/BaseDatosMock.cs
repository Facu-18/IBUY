using IBUY.Shared.DTO;

namespace IBUY.Servicios.Mocks
{
    /// <summary>
    /// Base de datos simulada, en memoria. Se registra como singleton, asi que vive
    /// mientras dure la sesion del navegador: al recargar la aplicacion los datos
    /// vuelven al estado inicial.
    /// </summary>
    public class BaseDatosMock
    {
        public const string TipoCompradora = "Compradora";

        public const string TipoProveedora = "Proveedora";

        public const string EstadoBorrador = "Borrador";

        /// <summary>Unico estado que hace visible una nota en el marketplace.</summary>
        public const string EstadoPublicada = "Publicada";

        public const string EstadoCerrada = "Cerrada";

        /// <summary>Empresa sobre la que trabaja la sesion simulada.</summary>
        public const int EmpresaCompradoraId = 1;

        /// <summary>Demora artificial para que las pantallas puedan mostrar su estado de carga.</summary>
        public static readonly TimeSpan Retardo = TimeSpan.FromMilliseconds(300);

        public List<EmpresaDTO> Empresas { get; } = [];

        public List<UsuarioDTO> Usuarios { get; } = [];

        public List<ProductoDTO> Productos { get; } = [];

        public List<NotaPedidoDTO> NotasPedido { get; } = [];

        public BaseDatosMock()
        {
            Sembrar();
        }

        public int SiguienteId<T>(List<T> coleccion) where T : DtoBase
        {
            return coleccion.Count == 0 ? 1 : coleccion.Max(elemento => elemento.Id) + 1;
        }

        private void Sembrar()
        {
            Empresas.AddRange(
            [
                new EmpresaDTO
                {
                    Id = 1,
                    RazonSocial = "Constructora del Sur SA",
                    Rubro = "Construccion",
                    Cuit = "30-71234567-8",
                    Email = "compras@constructoradelsur.com",
                    Tipo = TipoCompradora,
                    Activo = true
                },
                new EmpresaDTO
                {
                    Id = 2,
                    RazonSocial = "Ferreteria Industrial SRL",
                    Rubro = "Ferreteria",
                    Cuit = "30-70987654-3",
                    Email = "ventas@ferreteriaindustrial.com",
                    Tipo = TipoProveedora,
                    Activo = true
                },
                new EmpresaDTO
                {
                    Id = 3,
                    RazonSocial = "Aceros del Litoral SA",
                    Rubro = "Metalurgia",
                    Cuit = "30-69876543-1",
                    Email = "comercial@acerosdellitoral.com",
                    Tipo = TipoProveedora,
                    Activo = true
                },
                new EmpresaDTO
                {
                    Id = 4,
                    RazonSocial = "Distribuidora Pampa SRL",
                    Rubro = "Materiales de construccion",
                    Cuit = "30-68765432-9",
                    Email = "info@distribuidorapampa.com",
                    Tipo = TipoProveedora,
                    Activo = true
                },
                new EmpresaDTO
                {
                    Id = 5,
                    RazonSocial = "Constructora Andina SRL",
                    Rubro = "Construccion",
                    Cuit = "30-67654321-5",
                    Email = "compras@constructoraandina.com",
                    Tipo = TipoCompradora,
                    Activo = true
                },
                new EmpresaDTO
                {
                    Id = 6,
                    RazonSocial = "Grupo Edilicio Norte SA",
                    Rubro = "Construccion",
                    Cuit = "30-66543210-7",
                    Email = "abastecimiento@edilicionorte.com",
                    Tipo = TipoCompradora,
                    Activo = true
                }
            ]);

            // Cada usuario apunta a una empresa existente de la lista de arriba.
            Usuarios.AddRange(
            [
                new UsuarioDTO
                {
                    Id = 1,
                    Nombre = "Ana Gomez",
                    Email = "ana.gomez@constructoradelsur.com",
                    Rol = "Administradora",
                    Estado = true,
                    EmpresaId = 1
                },
                new UsuarioDTO
                {
                    Id = 2,
                    Nombre = "Martin Rossi",
                    Email = "martin.rossi@constructoradelsur.com",
                    Rol = "Comprador",
                    Estado = true,
                    EmpresaId = 1
                },
                new UsuarioDTO
                {
                    Id = 3,
                    Nombre = "Lucia Ferrer",
                    Email = "lucia.ferrer@constructoradelsur.com",
                    Rol = "Aprobadora",
                    Estado = true,
                    EmpresaId = 1
                },
                new UsuarioDTO
                {
                    Id = 4,
                    Nombre = "Diego Paz",
                    Email = "diego.paz@ferreteriaindustrial.com",
                    Rol = "Vendedor",
                    Estado = true,
                    EmpresaId = 2
                },
                new UsuarioDTO
                {
                    Id = 5,
                    Nombre = "Sofia Nunez",
                    Email = "sofia.nunez@acerosdellitoral.com",
                    Rol = "Vendedora",
                    Estado = true,
                    EmpresaId = 3
                },
                new UsuarioDTO
                {
                    Id = 6,
                    Nombre = "Pablo Diaz",
                    Email = "pablo.diaz@distribuidorapampa.com",
                    Rol = "Vendedor",
                    Estado = false,
                    EmpresaId = 4
                }
            ]);

            SembrarProductos();
            SembrarNotasPedido();
        }

        private void SembrarProductos()
        {
            // Cada producto pertenece al catalogo de una empresa compradora (1, 5 y 6).
            Productos.AddRange(
            [
                new ProductoDTO { Id = 1, Nombre = "Cemento Portland 50 kg", Categoria = "Aridos y cementos", UnidadMedida = "bolsa", EmpresaId = 1 },
                new ProductoDTO { Id = 2, Nombre = "Hierro nervurado del 8", Categoria = "Hierros", UnidadMedida = "barra", EmpresaId = 1 },
                new ProductoDTO { Id = 3, Nombre = "Ladrillo hueco 12x18x33", Categoria = "Mamposteria", UnidadMedida = "unidad", EmpresaId = 1 },
                new ProductoDTO { Id = 4, Nombre = "Arena fina", Categoria = "Aridos y cementos", UnidadMedida = "m3", EmpresaId = 5 },
                new ProductoDTO { Id = 5, Nombre = "Membrana asfaltica 4 mm", Categoria = "Impermeabilizantes", UnidadMedida = "rollo", EmpresaId = 5 },
                new ProductoDTO { Id = 6, Nombre = "Cal hidratada 25 kg", Categoria = "Aridos y cementos", UnidadMedida = "bolsa", EmpresaId = 6 },
                new ProductoDTO { Id = 7, Nombre = "Cano PVC 110 mm", Categoria = "Sanitarios", UnidadMedida = "unidad", EmpresaId = 6 }
            ]);
        }

        private void SembrarNotasPedido()
        {
            var hoy = DateTime.Today;

            // Las notas cubren los cuatro casos que el marketplace tiene que distinguir:
            // propia, borrador, cerrada y vencida no se publican; solo quedan las vigentes de terceros.
            NotasPedido.AddRange(
            [
                Nota(1, 1, EstadoPublicada, hoy.AddDays(-3), hoy.AddDays(12), "Obra Circunvalacion, entrega en planta.",
                    [Item(1, 1, 1, 100m), Item(2, 1, 2, 60m)]),

                Nota(2, 1, EstadoBorrador, hoy.AddDays(-1), hoy.AddDays(20), "Falta aprobacion interna.",
                    [Item(3, 2, 3, 2000m)]),

                Nota(3, 5, EstadoPublicada, hoy.AddDays(-5), hoy.AddDays(8), "Requiere flete incluido.",
                    [Item(4, 3, 4, 25m), Item(5, 3, 5, 40m)]),

                Nota(4, 6, EstadoPublicada, hoy.AddDays(-10), hoy.AddDays(2), "Entrega parcial aceptada.",
                    [Item(6, 4, 6, 80m), Item(7, 4, 7, 150m)]),

                Nota(5, 6, EstadoPublicada, hoy.AddDays(-30), hoy.AddDays(-4), "Convocatoria vencida.",
                    [Item(8, 5, 6, 50m)]),

                Nota(6, 5, EstadoCerrada, hoy.AddDays(-25), hoy.AddDays(-10), "Adjudicada.",
                    [Item(9, 6, 4, 15m)]),

                Nota(7, 5, EstadoPublicada, hoy.AddDays(-1), hoy.AddDays(20), "Compra programada para el proximo mes.",
                    [Item(10, 7, 4, 60m)])
            ]);
        }

        private NotaPedidoDTO Nota(int id, int empresaId, string estado, DateTime emision, DateTime vencimiento,
            string observaciones, List<ItemNotaDTO> items)
        {
            return new NotaPedidoDTO
            {
                Id = id,
                EmpresaId = empresaId,
                EmpresaNombre = Empresas.First(empresa => empresa.Id == empresaId).RazonSocial,
                Estado = estado,
                FechaEmision = emision,
                FechaVencimiento = vencimiento,
                Observaciones = observaciones,
                Items = items
            };
        }

        private ItemNotaDTO Item(int id, int notaPedidoId, int productoId, decimal cantidad)
        {
            var producto = Productos.First(p => p.Id == productoId);

            return new ItemNotaDTO
            {
                Id = id,
                NotaPedidoId = notaPedidoId,
                ProductoId = productoId,
                ProductoNombre = producto.Nombre,
                UnidadMedida = producto.UnidadMedida,
                CantidadSolicitada = cantidad
            };
        }
    }
}
