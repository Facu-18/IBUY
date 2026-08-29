using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/usuario")]
    public class NecesidadController : Controller
    {
        private readonly IRepositorio<Necesidad> repositorio;

        public NecesidadController(IRepositorio<Necesidad> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Necesidad>>> Get()
        {
            var necesidades = await repositorio.Select();
            return Ok(necesidades);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NecesidadDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró la necesidad de id: {id}");
            }

            var necesidad = await repositorio.SelectById(id);

            NecesidadDTO dto = new NecesidadDTO();
            dto.Nombre = necesidad!.Nombre;
            dto.Descripcion = necesidad.Descripcion;
            dto.Estado = necesidad.Estado;


            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(NecesidadDTO necesidadDTO)
        {
            Necesidad necesidad = new Necesidad();
            necesidad.Nombre = necesidadDTO.Nombre;
            necesidad.Descripcion = necesidadDTO.Descripcion;
            necesidad.Estado = necesidadDTO.Estado;


            await repositorio.Insert(necesidad);

            return Ok(necesidad);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, NecesidadDTO necesidadDTO)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            necesidad.Nombre = necesidadDTO.Nombre;
            necesidad.Descripcion = necesidadDTO.Descripcion;
            necesidad.Estado = necesidadDTO.Estado;

            var resultado = await repositorio.Update(necesidad);
            return Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe la necesidad con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }
    }
}
