using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;


namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/deposito")]

    public class DepositoController : Controller
    {

        private readonly IRepositorio<Deposito> repositorio;

        public DepositoController(IRepositorio<Deposito > repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Deposito>>> Get()
        {
            var depositos= await repositorio.Select();
            return Ok(depositos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepositoDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el deposito de id: {id}");
            }

            var deposito = await repositorio.SelectById(id);

            DepositoDTO dto = new DepositoDTO();
            dto.Nombre = deposito!.Nombre;
            dto.Tipo = deposito.Tipo;
            dto.Direccion = deposito.Direccion;
            dto.Activo = deposito.Activo;
            dto.EmpresaId = deposito.EmpresaId;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(DepositoDTO depositoDTO)
        {
            Deposito deposito = new Deposito();
            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo    ;
            deposito.EmpresaId = depositoDTO.EmpresaId;

            await repositorio.Insert(deposito);

            return Ok(deposito);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, DepositoDTO depositoDTO)
        {
            var deposito = await repositorio.SelectById(id);
            if (deposito is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;

            var resultado = await repositorio.Update(deposito);
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



