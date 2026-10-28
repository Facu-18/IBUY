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

        /// <summary>Rol con el que se crean los usuarios de una empresa (registro y alta desde Usuarios).</summary>
        public const string Administrador = "Administrador";

        public static bool EsSuperAdmin(string? rol)
        {
            return string.Equals(rol, SuperAdmin, StringComparison.OrdinalIgnoreCase);
        }
    }
}
