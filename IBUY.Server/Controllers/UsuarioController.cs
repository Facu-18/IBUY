using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Server.Seguridad;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;


namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/usuario")]

    public class UsuarioController : Controller
    {

        private readonly IUsuarioRepositorio repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;

        public UsuarioController(
            IUsuarioRepositorio repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Deposito> depositoRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.depositoRepositorio = depositoRepositorio;
        }

        /// <summary>
        /// Listado de usuarios sin exponer la contraseña (ni su hash): se proyecta a UsuarioDTO,
        /// que no declara el campo Contrasena.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> Get()
        {
            var usuarios = await repositorio.Select();

            var dtos = usuarios
                .Select(u => new UsuarioDTO
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Rol = u.Rol,
                    Estado = u.Estado,
                    EmpresaId = u.EmpresaId
                })
                .ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Opciones mínimas (Id + Nombre) de los usuarios activos de una empresa,
        /// para poblar combos sin exponer email ni contraseña.
        /// </summary>
        [HttpGet("empresa/{empresaId:int}")]
        public async Task<ActionResult<List<UsuarioOpcionDTO>>> GetOpcionesPorEmpresa(int empresaId)
        {
            if (!await empresaRepositorio.Existe(empresaId))
            {
                return NotFound($"No existe la empresa de id: {empresaId}");
            }

            var usuarios = (await repositorio.Select())
                .Where(u => u.EmpresaId == empresaId && u.Estado)
                .OrderBy(u => u.Nombre)
                .Select(u => new UsuarioOpcionDTO { Id = u.Id, Nombre = u.Nombre })
                .ToList();

            return Ok(usuarios);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el usuario de id: {id}");
            }

            var usuario = await repositorio.SelectById(id);

            UsuarioDTO dto = new UsuarioDTO();
            dto.Nombre = usuario!.Nombre;
            dto.Email = usuario.Email;
            dto.Rol = usuario.Rol;
            dto.Estado = usuario.Estado;
            dto.EmpresaId = usuario.EmpresaId;
            dto.DepositoIds = await repositorio.ObtenerDepositoIds(id);


            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(CrearUsuarioDTO usuarioDTO)
        {
            var depositoIds = usuarioDTO.DepositoIds.Distinct().ToList();

            var error = await ValidarRelaciones(usuarioDTO.EmpresaId, depositoIds);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Usuario usuario = new Usuario();
            usuario.Nombre = usuarioDTO.Nombre;
            usuario.Email = usuarioDTO.Email;
            usuario.Contrasena = HashContrasenas.Hashear(usuarioDTO.Contrasena);
            usuario.Rol = usuarioDTO.Rol;
            usuario.Estado = usuarioDTO.Estado;
            usuario.EmpresaId = usuarioDTO.EmpresaId;


            await repositorio.InsertarConDepositos(usuario, depositoIds);

            return Ok(usuario.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, CrearUsuarioDTO usuarioDTO)
        {
            var usuario = await repositorio.SelectById(id);
            if (usuario is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var depositoIds = usuarioDTO.DepositoIds.Distinct().ToList();

            var error = await ValidarRelaciones(usuarioDTO.EmpresaId, depositoIds);
            if (error is not null)
            {
                return BadRequest(error);
            }

            usuario.Nombre = usuarioDTO.Nombre;
            usuario.Contrasena = HashContrasenas.Hashear(usuarioDTO.Contrasena);
            usuario.Email = usuarioDTO.Email;
            usuario.Rol = usuarioDTO.Rol;
            usuario.Estado = usuarioDTO.Estado;
            usuario.EmpresaId = usuarioDTO.EmpresaId;

            var resultado = await repositorio.ActualizarConDepositos(usuario, depositoIds);
            return Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe el usuario con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }

        /// <summary>
        /// La empresa debe existir y cada depósito debe existir y pertenecer a esa misma empresa.
        /// Si un depósito ya tiene otro responsable, queda reasignado a este usuario (un solo
        /// responsable por depósito).
        /// </summary>
        private async Task<string?> ValidarRelaciones(int empresaId, List<int> depositoIds)
        {
            if (!await empresaRepositorio.Existe(empresaId))
            {
                return $"No existe la empresa de id: {empresaId}";
            }

            foreach (var depositoId in depositoIds)
            {
                if (!await depositoRepositorio.Existe(depositoId))
                {
                    return $"No existe el depósito de id: {depositoId}";
                }

                var deposito = await depositoRepositorio.SelectById(depositoId);
                if (deposito!.EmpresaId != empresaId)
                {
                    return $"El depósito de id: {depositoId} no pertenece a la empresa del usuario.";
                }
            }

            return null;
        }
    }
}
