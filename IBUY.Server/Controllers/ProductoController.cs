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

        public ProductoController(IRepositorio<Producto> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Producto>>> Get()
        {
            var productos = await repositorio.Select();
            return Ok(productos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el producto de id: {id}");
            }

            var producto = await repositorio.SelectById(id);

            ProductoDTO dto = new ProductoDTO();
            dto.Nombre = producto!.Nombre;
            dto.Descripcion = producto.Descripcion;
            dto.Categoria = producto.Categoria;
            dto.EmpresaId = producto.EmpresaId;

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(ProductoDTO productoDTO)
        {
            Producto producto = new Producto();
            producto.Nombre = productoDTO.Nombre;
            producto.Descripcion = productoDTO.Descripcion;
            producto.Categoria = productoDTO.Categoria;
            producto.EmpresaId = productoDTO.EmpresaId;

            await repositorio.Insert(producto);

            return Ok(producto.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, ProductoDTO productoDTO)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var producto = await repositorio.SelectById(id);

            producto!.Nombre = productoDTO.Nombre;
            producto.Descripcion = productoDTO.Descripcion;
            producto.Categoria = productoDTO.Categoria;
            producto.EmpresaId = productoDTO.EmpresaId;

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