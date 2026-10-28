using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;

namespace IBUY.Server.Seguridad
{
    /// <summary>
    /// Crea el usuario SuperAdmin al iniciar el servidor si todavía no existe ninguno.
    /// Los datos salen de la sección "SuperAdmin" de la configuración (Nombre, Email, Contrasena);
    /// sin esa sección no hace nada. Como todo usuario necesita una empresa, el SuperAdmin
    /// pertenece a una empresa de plataforma "IBUY" que se crea junto con él.
    /// </summary>
    public static class SembradoSuperAdmin
    {
        public static async Task EjecutarAsync(IServiceProvider servicios, IConfiguration configuracion, ILogger logger)
        {
            var seccion = configuracion.GetSection("SuperAdmin");
            var email = seccion["Email"];
            var contrasena = seccion["Contrasena"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(contrasena))
            {
                return;
            }

            try
            {
                using var scope = servicios.CreateScope();
                var repositorio = scope.ServiceProvider.GetRequiredService<IUsuarioRepositorio>();

                if (await repositorio.ExisteConRol(Roles.SuperAdmin))
                {
                    return;
                }

                if (await repositorio.SelectPorEmail(email) is not null)
                {
                    logger.LogWarning("No se creó el SuperAdmin: ya existe un usuario con el email {Email}.", email);
                    return;
                }

                Empresa empresa = new Empresa();
                empresa.RazonSocial = "IBUY";
                empresa.Rubro = "Plataforma";
                empresa.Cuit = "00-00000000-0";
                empresa.Email = email;
                empresa.Tipo = "Plataforma";
                empresa.Activo = true;

                Usuario usuario = new Usuario();
                usuario.Nombre = seccion["Nombre"] ?? "Super administrador";
                usuario.Email = email;
                usuario.Contrasena = HashContrasenas.Hashear(contrasena);
                usuario.Rol = Roles.SuperAdmin;
                usuario.Estado = true;

                await repositorio.RegistrarConEmpresa(empresa, usuario);
                logger.LogInformation("Usuario SuperAdmin creado: {Email}.", email);
            }
            catch (Exception ex)
            {
                // Sin base disponible el servidor igual arranca; el SuperAdmin se crea en el próximo inicio.
                logger.LogWarning(ex, "No se pudo crear el usuario SuperAdmin.");
            }
        }
    }
}
