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
        private readonly IRepositorio<Usuario> usuarioRepositorio;

        public DepositoController(
            IRepositorio<Deposito> repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
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
            dto.UsuarioResponsableId = deposito.UsuarioResponsableId;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(DepositoDTO depositoDTO)
        {
            var error = await ValidarRelaciones(depositoDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Deposito deposito = new Deposito();
            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;
            deposito.UsuarioResponsableId = depositoDTO.UsuarioResponsableId;

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

            var error = await ValidarRelaciones(depositoDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;
            deposito.UsuarioResponsableId = depositoDTO.UsuarioResponsableId;

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

        /// <summary>
        /// Las FK exigen que la empresa y, si se informa, el usuario responsable existan.
        /// Sin este control, SQL Server rechaza el INSERT/UPDATE con un 500.
        /// </summary>
        private async Task<string?> ValidarRelaciones(DepositoDTO depositoDTO)
        {
            if (!await empresaRepositorio.Existe(depositoDTO.EmpresaId))
            {
                return $"No existe la empresa de id: {depositoDTO.EmpresaId}";
            }

            if (depositoDTO.UsuarioResponsableId.HasValue && !await usuarioRepositorio.Existe(depositoDTO.UsuarioResponsableId.Value))
            {
                return $"No existe el usuario de id: {depositoDTO.UsuarioResponsableId}";
            }

            return null;
        }
    }
}
