using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/empresa")]
    public class EmpresaController : Controller
    {
        private readonly IRepositorio<Empresa> repositorio;

        public EmpresaController(IRepositorio<Empresa> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Empresa>>> Get()
        {
            var empresas = await repositorio.Select();
            return Ok(empresas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmpresaDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró la empresa de id: {id}");
            }

            var empresa = await repositorio.SelectById(id);

            EmpresaDTO dto = new EmpresaDTO();
            dto.Id = empresa!.Id;
            dto.RazonSocial = empresa.RazonSocial;
            dto.Rubro = empresa.Rubro;
            dto.Cuit = empresa.Cuit;
            dto.Email = empresa.Email;
            dto.Tipo = empresa.Tipo;
            dto.Activo = empresa.Activo;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(EmpresaDTO empresaDTO)
        {
            Empresa empresa = new Empresa();
            empresa.RazonSocial = empresaDTO.RazonSocial;
            empresa.Rubro = empresaDTO.Rubro;
            empresa.Cuit = empresaDTO.Cuit;
            empresa.Email = empresaDTO.Email;
            empresa.Tipo = empresaDTO.Tipo;
            empresa.Activo = empresaDTO.Activo;

            await repositorio.Insert(empresa);

            return Ok(empresa.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, EmpresaDTO empresaDTO)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var empresa = await repositorio.SelectById(id);
            empresa!.RazonSocial = empresaDTO.RazonSocial;
            empresa.Rubro = empresaDTO.Rubro;
            empresa.Cuit = empresaDTO.Cuit;
            empresa.Email = empresaDTO.Email;
            empresa.Tipo = empresaDTO.Tipo;
            empresa.Activo = empresaDTO.Activo;

            var resultado = await repositorio.Update(empresa);

            return Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }
    }
}
