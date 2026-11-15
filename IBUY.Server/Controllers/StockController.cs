using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Server.Seguridad;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : Controller
    {
        private readonly IRepositorio<Stock> repositorio;

        private readonly IRepositorio<Deposito> depositoRepositorio;

        private readonly IRepositorio<Producto> productoRepositorio;

        private readonly IPermisosUsuario permisos;

        public StockController(
            IRepositorio<Stock> repositorio,
            IRepositorio<Deposito> depositoRepositorio,
            IRepositorio<Producto> productoRepositorio,
            IPermisosUsuario permisos)
        {
            this.repositorio = repositorio;
            this.depositoRepositorio = depositoRepositorio;
            this.productoRepositorio = productoRepositorio;
            this.permisos = permisos;
        }

        /// <summary>
        /// Stock de los depósitos que el usuario puede ver, con nombres, mínimo y estado
        /// (Normal, Bajo o Agotado). Filtros opcionales: depósito (si no lo ve, 403),
        /// producto y solo críticos (bajo o agotado).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<StockListadoDTO>>> Get(
            [FromQuery] int? depositoId,
            [FromQuery] int? productoId,
            [FromQuery] bool soloCriticos,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is null && depositoId.HasValue)
            {
                acceso = permisos.ExigirDeposito(depositoId.Value);
            }

            if (acceso is not null)
            {
                return acceso;
            }

            var stocks = (await repositorio.Select())
                .Where(s => permisos.PuedeVerDeposito(s.DepositoId)
                    && (!depositoId.HasValue || s.DepositoId == depositoId.Value)
                    && (!productoId.HasValue || s.ProductoId == productoId.Value))
                .ToList();

            var listado = await AListado(stocks);
            if (soloCriticos)
            {
                listado = [.. listado.Where(s => EstadosStock.EsCritico(s.Estado))];
            }

            return Ok(listado
                .OrderBy(s => s.DepositoNombre)
                .ThenBy(s => s.ProductoNombre)
                .ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StockListadoDTO>> GetById(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            var stock = await repositorio.SelectById(id);
            if (stock is null)
            {
                return NotFound($"No se encontró el registro de stock con id: {id}");
            }

            acceso = permisos.ExigirDeposito(stock.DepositoId);
            if (acceso is not null)
            {
                return acceso;
            }

            return Ok((await AListado([stock]))[0]);
        }

        /// <summary>
        /// Cambia solo la cantidad mínima (no la actual: el stock se mueve con remitos).
        /// La puede cambiar quien ve el depósito: SuperAdmin, Ventas de la empresa o el
        /// Encargado del depósito. Negativa: 400.
        /// </summary>
        [HttpPut("{id:int}/minimo")]
        public async Task<ActionResult<bool>> PutMinimo(
            int id,
            CantidadMinimaDTO dto,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (dto.CantidadMinima < 0)
            {
                return BadRequest("La cantidad mínima no puede ser negativa.");
            }

            var stock = await repositorio.SelectById(id);
            if (stock is null)
            {
                return NotFound($"No se encontró el registro de stock con id: {id}");
            }

            acceso = permisos.ExigirDeposito(stock.DepositoId);
            if (acceso is not null)
            {
                return acceso;
            }

            stock.CantidadMinima = dto.CantidadMinima;
            return Ok(await repositorio.Update(stock));
        }

        /// <summary>
        /// Carga inicial del stock de un producto en un depósito: cantidad actual y mínima.
        /// Es solo el alta: si ese producto ya tiene stock en el depósito es 400, porque
        /// desde ahí la cantidad se mueve con remitos. Lo puede hacer quien ve el depósito;
        /// el producto tiene que ser de la misma empresa que el depósito.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Post(
            StockDTO stockDTO,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (stockDTO.CantidadActual < 0 || stockDTO.CantidadMinima < 0)
            {
                return BadRequest("La cantidad actual y la cantidad mínima no pueden ser negativas.");
            }

            var deposito = await depositoRepositorio.SelectById(stockDTO.DepositoId);
            if (deposito is null)
            {
                return BadRequest($"No existe el depósito de id: {stockDTO.DepositoId}");
            }

            acceso = permisos.ExigirDeposito(deposito.Id);
            if (acceso is not null)
            {
                return acceso;
            }

            var producto = await productoRepositorio.SelectById(stockDTO.ProductoId);
            if (producto is null || producto.EmpresaId != deposito.EmpresaId)
            {
                return BadRequest($"No existe el producto de id: {stockDTO.ProductoId} en la empresa del depósito.");
            }

            var existente = (await repositorio.Select())
                .Any(s => s.DepositoId == deposito.Id && s.ProductoId == producto.Id);
            if (existente)
            {
                return BadRequest($"{producto.Nombre} ya tiene stock cargado en {deposito.Nombre}. Desde la carga inicial, la cantidad se mueve con remitos.");
            }

            Stock stock = new Stock
            {
                CantidadActual = stockDTO.CantidadActual,
                CantidadMinima = stockDTO.CantidadMinima,
                DepositoId = deposito.Id,
                ProductoId = producto.Id
            };

            await repositorio.Insert(stock);

            return Ok(stock.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, StockDTO stockDTO)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var stock = await repositorio.SelectById(id);
            stock!.CantidadActual = stockDTO.CantidadActual;
            stock.DepositoId = stockDTO.DepositoId;
            stock.ProductoId = stockDTO.ProductoId;

            var resultado = await repositorio.Update(stock);

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

        /// <summary>Une stock con depósito y producto y calcula el estado.</summary>
        private async Task<List<StockListadoDTO>> AListado(List<Stock> stocks)
        {
            var depositos = (await depositoRepositorio.Select()).ToDictionary(d => d.Id, d => d.Nombre);
            var productos = (await productoRepositorio.Select()).ToDictionary(p => p.Id);

            return [.. stocks.Select(s => new StockListadoDTO
            {
                Id = s.Id,
                DepositoId = s.DepositoId,
                DepositoNombre = depositos.TryGetValue(s.DepositoId, out var deposito) ? deposito : string.Empty,
                ProductoId = s.ProductoId,
                ProductoNombre = productos.TryGetValue(s.ProductoId, out var producto) ? producto.Nombre : string.Empty,
                Categoria = producto?.Categoria ?? string.Empty,
                CantidadActual = s.CantidadActual,
                CantidadMinima = s.CantidadMinima,
                Estado = EstadosStock.Calcular(s.CantidadActual, s.CantidadMinima)
            })];
        }
    }
}
