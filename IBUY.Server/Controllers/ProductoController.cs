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
        private readonly IRepositorio<IBUY.BD.Datos.Entity.Producto> repositorio;

        public ProductoController(IRepositorio<IBUY.BD.Datos.Entity.Producto> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductoDTO>>> Get()
        {
            var productos = await repositorio.Select();
            return Ok(productos);
        }

        [HttpGet ("{id:int}")]

        public async Task<ActionResult<List<ProductoDTO>>> GetByEmpresaId(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró la empresa con id: {id}");
            }

            var productos = await repositorio.SelectById(id);

            ProductoDTO dto = new ProductoDTO();
            dto.Nombre = productos.Nombre;
            dto.Descripcion = productos.Descripcion;
            dto.Categoria = productos.Categoria;
            dto.EmpresaId = productos.EmpresaId;

            return Ok(dto);
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
            IBUY.BD.Datos.Entity.Producto producto =
                new IBUY.BD.Datos.Entity.Producto();

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

            if (producto == null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            producto.Nombre = productoDTO.Nombre;
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

    public class ProductoDTO
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Categoria { get; set; }
        public int EmpresaId { get; set; }
    }
}