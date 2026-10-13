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
        private readonly IRepositorio<Usuario> usuarioRepositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;

        public AuthController(
            IRepositorio<Usuario> usuarioRepositorio,
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
            var usuarios = await usuarioRepositorio.Select();
            var usuario = usuarios.FirstOrDefault(u => u.Email.Equals(loginDTO.Email, StringComparison.OrdinalIgnoreCase));

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
    }
}
