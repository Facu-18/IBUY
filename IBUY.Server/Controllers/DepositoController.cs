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
        private readonly IRepositorio<Empresa> empresaRepositorio;

        public DepositoController(IRepositorio<Deposito> repositorio, IRepositorio<Empresa> empresaRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Deposito>>> Get()
        {
            var depositos = await repositorio.Select();
            return Ok(depositos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepositoDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el depósito de id: {id}");
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
            // La FK exige que la empresa exista: sin este control, SQL Server rechaza
            // el INSERT y el error sale como un 500 en vez de un mensaje util.
            if (!await empresaRepositorio.Existe(depositoDTO.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {depositoDTO.EmpresaId}");
            }

            Deposito deposito = new Deposito();
            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;

            await repositorio.Insert(deposito);

            return Ok(deposito.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, DepositoDTO depositoDTO)
        {
            var deposito = await repositorio.SelectById(id);
            if (deposito is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            if (!await empresaRepositorio.Existe(depositoDTO.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {depositoDTO.EmpresaId}");
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
                return NotFound($"No existe el depósito con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }
    }
}
