using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/cotizacion")]
    public class CotizacionController : Controller
    {
        private readonly ICotizacionRepositorio repositorio;
        private readonly IRepositorio<NotaPedido> notaPedidoRepositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<ItemNota> itemNotaRepositorio;

        public CotizacionController(
            ICotizacionRepositorio repositorio,
            IRepositorio<NotaPedido> notaPedidoRepositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<ItemNota> itemNotaRepositorio)
        {
            this.repositorio = repositorio;
            this.notaPedidoRepositorio = notaPedidoRepositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.itemNotaRepositorio = itemNotaRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Cotizacion>>> Get()
        {
            var cotizaciones = await repositorio.Select();
            return Ok(cotizaciones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CotizacionDTO>> GetById(int id)
        {
            var (cotizacion, items) = await repositorio.ObtenerDetalle(id);
            if (cotizacion is null)
            {
                return NotFound($"No se encontró la cotización de id: {id}");
            }

            return Ok(AMapaDTO(cotizacion, items));
        }

        [HttpGet("nota/{notaPedidoId:int}")]
        public async Task<ActionResult<List<Cotizacion>>> GetPorNotaPedido(int notaPedidoId)
        {
            if (!await notaPedidoRepositorio.Existe(notaPedidoId))
            {
                return NotFound($"No existe la nota de pedido de id: {notaPedidoId}");
            }

            var cotizaciones = await repositorio.ObtenerPorNotaPedido(notaPedidoId);
            return Ok(cotizaciones);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(CotizacionDTO cotizacionDTO)
        {
            if (cotizacionDTO.Items is null || cotizacionDTO.Items.Count == 0)
            {
                return BadRequest("La cotización debe tener al menos un ítem.");
            }

            var error = await ValidarRelaciones(cotizacionDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Cotizacion cotizacion = new Cotizacion
            {
                Estado = cotizacionDTO.Estado,
                PlazoEntrega = cotizacionDTO.PlazoEntrega,
                FechaEnvio = cotizacionDTO.FechaEnvio,
                NotaPedidoId = cotizacionDTO.NotaPedidoId,
                EmpresaProveedoraId = cotizacionDTO.EmpresaProveedoraId
            };

            var items = cotizacionDTO.Items.Select(i => new ItemCotizacion
            {
                ItemNotaId = i.ItemNotaId,
                CantidadOfertada = i.CantidadOfertada,
                PrecioUnitario = i.PrecioUnitario,
                PrecioTotal = Math.Round(
                    i.CantidadOfertada * i.PrecioUnitario,
                    2,
                    MidpointRounding.AwayFromZero)
            }).ToList();

            await repositorio.InsertarConItems(cotizacion, items);

            return Ok(cotizacion.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, CotizacionDTO cotizacionDTO)
        {
            var cotizacion = await repositorio.SelectById(id);
            if (cotizacion is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            if (!await notaPedidoRepositorio.Existe(cotizacionDTO.NotaPedidoId))
            {
                return BadRequest($"No existe la nota de pedido de id: {cotizacionDTO.NotaPedidoId}");
            }

            if (!await empresaRepositorio.Existe(cotizacionDTO.EmpresaProveedoraId))
            {
                return BadRequest($"No existe la empresa proveedora de id: {cotizacionDTO.EmpresaProveedoraId}");
            }

            // Solo se actualiza el encabezado (incluye Estado): los items no se tocan aca.
            cotizacion.Estado = cotizacionDTO.Estado;
            cotizacion.PlazoEntrega = cotizacionDTO.PlazoEntrega;
            cotizacion.FechaEnvio = cotizacionDTO.FechaEnvio;
            cotizacion.NotaPedidoId = cotizacionDTO.NotaPedidoId;
            cotizacion.EmpresaProveedoraId = cotizacionDTO.EmpresaProveedoraId;

            var resultado = await repositorio.Update(cotizacion);
            return Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe la cotización con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }

        private static CotizacionDTO AMapaDTO(Cotizacion cotizacion, List<ItemCotizacion> items)
        {
            return new CotizacionDTO
            {
                Estado = cotizacion.Estado,
                PlazoEntrega = cotizacion.PlazoEntrega,
                FechaEnvio = cotizacion.FechaEnvio,
                NotaPedidoId = cotizacion.NotaPedidoId,
                EmpresaProveedoraId = cotizacion.EmpresaProveedoraId,
                Items = items.Select(i => new ItemCotizacionDTO
                {
                    ItemNotaId = i.ItemNotaId,
                    CantidadOfertada = i.CantidadOfertada,
                    PrecioUnitario = i.PrecioUnitario,
                    PrecioTotal = i.PrecioTotal
                }).ToList()
            };
        }

        /// <summary>
        /// La nota debe existir, estar publicada, no estar vencida y pertenecer a otra
        /// empresa. Además las FK exigen que la empresa proveedora y los items de nota
        /// referenciados existan. Devuelve null cuando esta todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(CotizacionDTO dto)
        {
            var notaPedido = await notaPedidoRepositorio.SelectById(dto.NotaPedidoId);
            if (notaPedido is null)
            {
                return $"No existe la nota de pedido de id: {dto.NotaPedidoId}";
            }

            if (notaPedido.Estado != EstadosNotaPedido.Publicada)
            {
                return "La nota de pedido no está publicada y no admite cotizaciones.";
            }

            if (notaPedido.FechaVencimiento.Date < DateTime.Today)
            {
                return "La nota de pedido está vencida y no admite cotizaciones.";
            }

            if (notaPedido.EmpresaId == dto.EmpresaProveedoraId)
            {
                return "La empresa proveedora no puede cotizar su propia nota de pedido.";
            }

            if (!await empresaRepositorio.Existe(dto.EmpresaProveedoraId))
            {
                return $"No existe la empresa proveedora de id: {dto.EmpresaProveedoraId}";
            }

            foreach (var item in dto.Items)
            {
                if (!await itemNotaRepositorio.Existe(item.ItemNotaId))
                {
                    return $"No existe el ítem de nota de pedido de id: {item.ItemNotaId}";
                }
            }

            return null;
        }
    }
}
