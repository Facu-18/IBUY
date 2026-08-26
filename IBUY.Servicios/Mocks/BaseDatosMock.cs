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

        /// <summary>Empresa sobre la que trabaja la sesion simulada.</summary>
        public const int EmpresaCompradoraId = 1;

        /// <summary>Demora artificial para que las pantallas puedan mostrar su estado de carga.</summary>
        public static readonly TimeSpan Retardo = TimeSpan.FromMilliseconds(300);

        public List<EmpresaDTO> Empresas { get; } = [];

        public List<UsuarioDTO> Usuarios { get; } = [];

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
        }
    }
}
