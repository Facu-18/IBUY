using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Server.Seguridad;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IUsuarioRepositorio usuarioRepositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;

        public AuthController(
            IUsuarioRepositorio usuarioRepositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Deposito> depositoRepositorio)
        {
            this.usuarioRepositorio = usuarioRepositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.depositoRepositorio = depositoRepositorio;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginRespuestaDTO>> Login(LoginDTO loginDTO)
        {
            var usuario = await usuarioRepositorio.SelectPorEmail(loginDTO.Email);

            if (usuario is null || !usuario.Estado || !HashContrasenas.Verificar(loginDTO.Contrasena, usuario.Contrasena))
            {
                return Unauthorized("Email o contraseña incorrectos.");
            }

            var empresa = await empresaRepositorio.SelectById(usuario.EmpresaId);

            var depositos = await depositoRepositorio.Select();
            var depositosACargo = depositos
                .Where(d => d.UsuarioResponsableId == usuario.Id)
                .Select(d => new DepositoResumenDTO { Id = d.Id, Nombre = d.Nombre })
                .ToList();

            LoginRespuestaDTO respuesta = new LoginRespuestaDTO
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                EmpresaId = usuario.EmpresaId,
                EmpresaRazonSocial = empresa?.RazonSocial ?? string.Empty,
                Depositos = depositosACargo
            };

            return Ok(respuesta);
        }

        /// <summary>
        /// Registro público: crea la empresa y su primer usuario (rol Administrador) en una sola
        /// transacción, sin exigir depósitos, y devuelve la sesión iniciada.
        /// </summary>
        [HttpPost("registro")]
        public async Task<ActionResult<LoginRespuestaDTO>> Registro(RegistroDTO registroDTO)
        {
            if (await usuarioRepositorio.SelectPorEmail(registroDTO.Email) is not null)
            {
                return BadRequest("Ya existe un usuario registrado con ese email.");
            }

            Empresa empresa = new Empresa();
            empresa.RazonSocial = registroDTO.RazonSocial;
            empresa.Rubro = registroDTO.Rubro;
            empresa.Cuit = registroDTO.Cuit;
            empresa.Email = registroDTO.EmailEmpresa;
            empresa.Activo = true;

            Usuario usuario = new Usuario();
            usuario.Nombre = registroDTO.Nombre;
            usuario.Email = registroDTO.Email;
            usuario.Contrasena = HashContrasenas.Hashear(registroDTO.Contrasena);
            usuario.Rol = Roles.Administrador;
            usuario.Estado = true;

            await usuarioRepositorio.RegistrarConEmpresa(empresa, usuario);

            LoginRespuestaDTO respuesta = new LoginRespuestaDTO
            {
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol,
                EmpresaId = empresa.Id,
                EmpresaRazonSocial = empresa.RazonSocial,
                Depositos = []
            };

            return Ok(respuesta);
        }
    }
}
