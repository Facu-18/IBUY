using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Seguridad
{
    /// <summary>
    /// Qué puede ver y hacer el usuario que llama a la API. El cliente lo identifica con el
    /// encabezado X-Usuario-Id; este helper busca en la base al usuario, su rol y sus depósitos.
    /// Atención: el encabezado lo manda el cliente y se puede falsificar, así que esto ordena los
    /// permisos de la aplicación pero no es seguridad real (sin JWT ni cookies).
    /// </summary>
    public interface IPermisosUsuario
    {
        /// <summary>Nombre del encabezado HTTP con el id del usuario.</summary>
        const string Encabezado = "X-Usuario-Id";

        /// <summary>
        /// Carga al usuario del encabezado. Devuelve null si todo está bien, 401 si falta el
        /// encabezado o el usuario no existe o está inactivo, y 403 si su rol no da acceso a nada.
        /// </summary>
        Task<ActionResult?> Autenticar(string? usuarioId);

        bool EsSuperAdmin { get; }

        /// <summary>Ventas o SuperAdmin: pueden gestionar usuarios, depósitos, productos y notas.</summary>
        bool PuedeGestionar { get; }

        /// <summary>SuperAdmin: todos. Ventas: los de su empresa. Encargado: los que tiene a cargo.</summary>
        bool PuedeVerDeposito(int depositoId);

        /// <summary>SuperAdmin: cualquiera. Los demás roles: solo la suya.</summary>
        bool PuedeVerEmpresa(int empresaId);

        /// <summary>Id y empresa del usuario autenticado.</summary>
        int UsuarioId { get; }

        int EmpresaId { get; }

        /// <summary>403 si el usuario no es Ventas ni SuperAdmin; null si puede gestionar.</summary>
        ActionResult? ExigirGestion();

        /// <summary>403 si no puede ver ese depósito; null si puede.</summary>
        ActionResult? ExigirDeposito(int depositoId);

        /// <summary>403 si no puede ver esa empresa; null si puede.</summary>
        ActionResult? ExigirEmpresa(int empresaId);
    }

    public class PermisosUsuario : IPermisosUsuario
    {
        private readonly IUsuarioRepositorio usuarioRepositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;

        private Usuario? usuario;
        private HashSet<int> depositosVisibles = [];

        public PermisosUsuario(IUsuarioRepositorio usuarioRepositorio, IRepositorio<Deposito> depositoRepositorio)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            this.depositoRepositorio = depositoRepositorio;
        }

        public bool EsSuperAdmin => Roles.EsSuperAdmin(usuario?.Rol);

        private bool EsVentas => Roles.EsVentas(usuario?.Rol);

        private bool EsEncargado => Roles.EsEncargado(usuario?.Rol);

        public bool PuedeGestionar => EsSuperAdmin || EsVentas;

        public int UsuarioId => usuario?.Id ?? 0;

        public int EmpresaId => usuario?.EmpresaId ?? 0;

        public async Task<ActionResult?> Autenticar(string? usuarioId)
        {
            // Llega como texto para que un valor no numérico también sea 401 (y no un 400 de binding).
            if (!int.TryParse(usuarioId, out int id))
            {
                return new ObjectResult($"Falta el encabezado {IPermisosUsuario.Encabezado} o no es un id válido.") { StatusCode = StatusCodes.Status401Unauthorized };
            }

            usuario = await usuarioRepositorio.SelectById(id);
            if (usuario is null || !usuario.Estado)
            {
                usuario = null;
                return new ObjectResult("El usuario del encabezado no existe o está inactivo.") { StatusCode = StatusCodes.Status401Unauthorized };
            }

            // Un rol que no es SuperAdmin, Ventas ni EncargadoDeposito no tiene acceso a nada.
            if (!EsSuperAdmin && !EsVentas && !EsEncargado)
            {
                return Prohibido("Tu rol no tiene acceso a este recurso.");
            }

            depositosVisibles = await CargarDepositosVisibles(usuario);
            return null;
        }

        public bool PuedeVerDeposito(int depositoId)
        {
            return depositosVisibles.Contains(depositoId);
        }

        public bool PuedeVerEmpresa(int empresaId)
        {
            return EsSuperAdmin || empresaId == usuario?.EmpresaId;
        }

        public ActionResult? ExigirGestion()
        {
            return PuedeGestionar ? null : Prohibido("Solo Ventas o SuperAdmin pueden hacer esta acción.");
        }

        public ActionResult? ExigirDeposito(int depositoId)
        {
            return PuedeVerDeposito(depositoId) ? null : Prohibido("No tenés acceso a ese depósito.");
        }

        public ActionResult? ExigirEmpresa(int empresaId)
        {
            return PuedeVerEmpresa(empresaId) ? null : Prohibido("No tenés acceso a esa empresa.");
        }

        private static ObjectResult Prohibido(string mensaje)
        {
            return new ObjectResult(mensaje) { StatusCode = StatusCodes.Status403Forbidden };
        }

        /// <summary>Ids de los depósitos que el usuario puede ver, según su rol.</summary>
        private async Task<HashSet<int>> CargarDepositosVisibles(Usuario u)
        {
            if (Roles.EsEncargado(u.Rol))
            {
                return (await usuarioRepositorio.ObtenerDepositoIds(u.Id)).ToHashSet();
            }

            var depositos = await depositoRepositorio.Select();
            return depositos
                .Where(d => EsSuperAdmin || d.EmpresaId == u.EmpresaId)
                .Select(d => d.Id)
                .ToHashSet();
        }
    }
}
