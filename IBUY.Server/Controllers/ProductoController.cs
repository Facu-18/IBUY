using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controller
{
    [ApiController]
    [Route("api/Producto")]
    public class ProductoController : Controller
    {
        private readonly IRepositorio<Producto> repositorio;

        public ProductoController(IRepositorio<Producto> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> Get()
        {
            var productos = await repositorio.Select();

            var productosDTO = productos.Select(producto => new ProductoDTO
            {
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Categoria = producto.Categoria,
                EmpresaId = producto.EmpresaId
            }).ToList();

            return Ok(productosDTO);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductoDTO>> GetById(int id)
        {
            var producto = await repositorio.SelectById(id);

            if (producto == null)
            {
                return NotFound($"No se encontró el producto de id: {id}");
            }

            ProductoDTO productoDTO = new ProductoDTO();

            productoDTO.Nombre = producto.Nombre;
            productoDTO.Descripcion = producto.Descripcion;
            productoDTO.Categoria = producto.Categoria;
            productoDTO.EmpresaId = producto.EmpresaId;

            return Ok(productoDTO);
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




