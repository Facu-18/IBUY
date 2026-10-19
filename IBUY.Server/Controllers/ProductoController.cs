using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/producto")]
    public class ProductoController : Controller
    {
        private readonly IRepositorio<Producto> repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;

        public ProductoController(IRepositorio<Producto> repositorio, IRepositorio<Empresa> empresaRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> Get([FromQuery] int? empresaId)
        {
            var productos = await repositorio.Select();

            if (empresaId.HasValue)
            {
                productos = productos.Where(p => p.EmpresaId == empresaId.Value).ToList();
            }

            var respuesta = productos.Select(p => new ProductoDTO
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                Categoria = p.Categoria,
                EmpresaId = p.EmpresaId
            })
            .OrderBy(p => p.Nombre)
            .ToList();

            return Ok(respuesta);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDTO>> GetById(int id, [FromQuery] int? empresaId)
        {
            var producto = await repositorio.SelectById(id);
            if (producto is null || (empresaId.HasValue && producto.EmpresaId != empresaId.Value))
            {
                return NotFound($"No se encontró el producto de id: {id}");
            }

            ProductoDTO dto = new ProductoDTO();
            dto.Id = producto.Id;
            dto.Nombre = producto.Nombre;
            dto.Descripcion = producto.Descripcion;
            dto.Categoria = producto.Categoria;
            dto.EmpresaId = producto.EmpresaId;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(ProductoDTO productoDTO)
        {
            if (!await empresaRepositorio.Existe(productoDTO.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {productoDTO.EmpresaId}");
            }

            Producto producto = new Producto();
            producto.Nombre = productoDTO.Nombre.Trim();
            producto.Descripcion = productoDTO.Descripcion?.Trim() ?? string.Empty;
            producto.Categoria = productoDTO.Categoria.Trim();
            producto.EmpresaId = productoDTO.EmpresaId;

            await repositorio.Insert(producto);

            return Ok(producto.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, ProductoDTO productoDTO)
        {
            var producto = await repositorio.SelectById(id);
            if (producto is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            if (producto.EmpresaId != productoDTO.EmpresaId)
            {
                return BadRequest("No se puede cambiar la empresa del producto.");
            }

            if (!await empresaRepositorio.Existe(productoDTO.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {productoDTO.EmpresaId}");
            }

            producto.Nombre = productoDTO.Nombre.Trim();
            producto.Descripcion = productoDTO.Descripcion?.Trim() ?? string.Empty;
            producto.Categoria = productoDTO.Categoria.Trim();

            var resultado = await repositorio.Update(producto);

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
