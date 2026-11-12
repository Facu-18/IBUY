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

        private readonly IPermisosUsuario permisos;

        public StockController(IRepositorio<Stock> repositorio, IPermisosUsuario permisos)
        {
            this.repositorio = repositorio;
            this.permisos = permisos;
        }

        [HttpGet]
        public async Task<ActionResult<List<Stock>>> Get(
            [FromQuery] int? depositoId,
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

            // Solo el stock de los depósitos que el usuario puede ver.
            var stocks = (await repositorio.Select())
                .Where(s => permisos.PuedeVerDeposito(s.DepositoId) && (!depositoId.HasValue || s.DepositoId == depositoId.Value))
                .ToList();

            return Ok(stocks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StockDTO>> GetById(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el registro de stock con id: {id}");
            }

            var stock = await repositorio.SelectById(id);

            acceso = permisos.ExigirDeposito(stock!.DepositoId);
            if (acceso is not null)
            {
                return acceso;
            }

            StockDTO dto = new StockDTO
            {
                CantidadActual = stock!.CantidadActual,
                DepositoId = stock.DepositoId,
                ProductoId = stock.ProductoId
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(StockDTO stockDTO)
        {
            Stock stock = new Stock
            {
                CantidadActual = stockDTO.CantidadActual,
                DepositoId = stockDTO.DepositoId,
                ProductoId = stockDTO.ProductoId
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
    }
}