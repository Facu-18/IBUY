using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/stock")]
    public class StockController : Controller
    {
        private readonly IRepositorio<Stock> repositorio;

        public StockController(IRepositorio<Stock> repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Stock>>> Get()
        {
            var stocks = await repositorio.Select();
            return Ok(stocks);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StockDTO>> GetById(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el registro de stock con id: {id}");
            }

            var stock = await repositorio.SelectById(id);

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