namespace IBUY.Shared.DTO
{
    /// <summary>
    /// Roles de usuario con significado en la aplicación. Son los textos que se guardan en
    /// la base; viven acá para que el cliente y el servidor usen los mismos valores.
    /// </summary>
    public static class Roles
    {
        /// <summary>Administra la plataforma: es el único que ve y gestiona todas las empresas.</summary>
        public const string SuperAdmin = "SuperAdmin";

        /// <summary>Gestiona su empresa (usuarios, depósitos, productos, notas de pedido) y ve todos sus depósitos.</summary>
        public const string Ventas = "Ventas";

        /// <summary>Ve y opera solo los depósitos que tiene a cargo.</summary>
        public const string EncargadoDeposito = "EncargadoDeposito";

        /// <summary>
        /// LEGADO: ya no es un rol válido. La migración RolesVentasEncargado lo reemplazó por
        /// Ventas o EncargadoDeposito y la API lo rechaza al dar de alta o editar usuarios.
        /// Se conserva solo para que el cliente siga compilando hasta que se actualice.
        /// </summary>
        public const string Administrador = "Administrador";

        /// <summary>Roles que se pueden asignar a un usuario desde la API (SuperAdmin solo lo crea el sembrado).</summary>
        public static readonly string[] Asignables = [Ventas, EncargadoDeposito];

        public static bool EsSuperAdmin(string? rol)
        {
            return string.Equals(rol, SuperAdmin, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EsVentas(string? rol)
        {
            return string.Equals(rol, Ventas, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EsEncargado(string? rol)
        {
            return string.Equals(rol, EncargadoDeposito, StringComparison.OrdinalIgnoreCase);
        }
    }
}
