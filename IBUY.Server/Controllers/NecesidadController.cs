using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/necesidad")]
    public class NecesidadController : Controller
    {
        private readonly IRepositorio<Necesidad> repositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;

        public NecesidadController(
            IRepositorio<Necesidad> repositorio,
            IRepositorio<Deposito> depositoRepositorio,
            IRepositorio<Producto> productoRepositorio)
        {
            this.repositorio = repositorio;
            this.depositoRepositorio = depositoRepositorio;
            this.productoRepositorio = productoRepositorio;
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
            dto.CantidadRequerida = necesidad!.CantidadRequerida;
            dto.FechaRequerida = necesidad.FechaRequerida;
            dto.Estado = necesidad.Estado;
            dto.DepositoId = necesidad.DepositoId;
            dto.ProductoId = necesidad.ProductoId;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(NecesidadDTO necesidadDTO)
        {
            var error = await ValidarRelaciones(necesidadDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Necesidad necesidad = new Necesidad();
            necesidad.CantidadRequerida = necesidadDTO.CantidadRequerida;
            necesidad.FechaRequerida = necesidadDTO.FechaRequerida;
            necesidad.Estado = necesidadDTO.Estado;
            necesidad.DepositoId = necesidadDTO.DepositoId;
            necesidad.ProductoId = necesidadDTO.ProductoId;

            await repositorio.Insert(necesidad);

            return Ok(necesidad.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, NecesidadDTO necesidadDTO)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var error = await ValidarRelaciones(necesidadDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            necesidad.CantidadRequerida = necesidadDTO.CantidadRequerida;
            necesidad.FechaRequerida = necesidadDTO.FechaRequerida;
            necesidad.Estado = necesidadDTO.Estado;
            necesidad.DepositoId = necesidadDTO.DepositoId;
            necesidad.ProductoId = necesidadDTO.ProductoId;

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

        /// <summary>
        /// Las FK exigen que el deposito y el producto existan. Sin este control,
        /// SQL Server rechaza el INSERT y el error sale como un 500 en vez de un
        /// mensaje util. Devuelve null cuando esta todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(NecesidadDTO necesidadDTO)
        {
            if (!await depositoRepositorio.Existe(necesidadDTO.DepositoId))
            {
                return $"No existe el depósito de id: {necesidadDTO.DepositoId}";
            }

            if (!await productoRepositorio.Existe(necesidadDTO.ProductoId))
            {
                return $"No existe el producto de id: {necesidadDTO.ProductoId}";
            }

            return null;
        }
    }
}
