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

        [HttpPost]
        public async Task<ActionResult<int>> Post(CrearUsuarioDTO usuarioDTO)
        {
            Usuario usuario = new Usuario();
            usuario.Nombre = usuarioDTO.Nombre;
            usuario.Email = usuarioDTO.Email;
            usuario.Contrasena = usuarioDTO.Contrasena;
            usuario.Rol = usuarioDTO.Rol;
            usuario.Estado = usuarioDTO.Estado;
            usuario.EmpresaId = usuarioDTO.EmpresaId;
            

            await repositorio.Insert(usuario);

            return Ok(usuario);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, CrearUsuarioDTO usuarioDTO)
        {
            var usuario = await repositorio.SelectById(id);
            if (usuario is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            usuario.Nombre = usuarioDTO.Nombre;
            usuario.Contrasena = usuarioDTO.Contrasena;
            usuario.Email = usuarioDTO.Email;
            usuario.Rol = usuarioDTO.Rol;
            usuario.Estado = usuarioDTO.Estado;
            usuario.EmpresaId = usuarioDTO.EmpresaId;

            var resultado = await repositorio.Update(usuario);
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
    }
}


    

