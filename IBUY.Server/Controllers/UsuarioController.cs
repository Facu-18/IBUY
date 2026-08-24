using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;


namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/usuario")]

    public class UsuarioController : Controller
    {

        private readonly IRepositorio<Usuario> repositorio;

        public UsuarioController(IRepositorio<Usuario> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> Get()
        {
            var usuarios = await repositorio.Select();
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


            return Ok(dto);
        }

    }
}
