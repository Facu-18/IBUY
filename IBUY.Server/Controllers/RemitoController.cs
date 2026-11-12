using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/remito")]
    public class RemitoController : Controller
    {
        private readonly IRemitoRepositorio repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Usuario> usuarioRepositorio;
        private readonly IRepositorio<Cotizacion> cotizacionRepositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;

        public RemitoController(
            IRemitoRepositorio repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio,
            IRepositorio<Cotizacion> cotizacionRepositorio,
            IRepositorio<Deposito> depositoRepositorio,
            IRepositorio<Producto> productoRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
            this.cotizacionRepositorio = cotizacionRepositorio;
            this.depositoRepositorio = depositoRepositorio;
            this.productoRepositorio = productoRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Remito>>> Get()
        {
            var remitos = await repositorio.Select();
            return Ok(remitos);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RemitoDTO>> GetById(int id)
        {
            var (remito, items) = await repositorio.ObtenerDetalle(id);
            if (remito is null)
            {
                return NotFound($"No se encontró el remito de id: {id}");
            }

            RemitoDTO dto = new RemitoDTO
            {
                Id = remito.Id,
                Tipo = remito.Tipo,
                EmpresaOrigen = remito.EmpresaOrigen,
                Numero = remito.Numero,
                FechaEmision = remito.FechaEmision,
                FechaRecepcion = remito.FechaRecepcion,
                Estado = remito.Estado,
                EmpresaId = remito.EmpresaId,
                UsuarioId = remito.UsuarioId,
                CotizacionId = remito.CotizacionId,
                DepositoOrigenId = remito.DepositoOrigenId,
                DepositoDestinoId = remito.DepositoDestinoId,
                Items = items.Select(i => new ItemRemitoDTO
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    Unidad = i.Unidad
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(RemitoDTO remitoDTO)
        {
            if (remitoDTO.Items is null || remitoDTO.Items.Count == 0)
            {
                return BadRequest("El remito debe tener al menos un ítem.");
            }

            var errorTipo = ValidarTipoYDepositos(remitoDTO, out string tipoCanonico);
            if (errorTipo is not null)
            {
                return BadRequest(errorTipo);
            }

            var errorRelaciones = await ValidarRelaciones(remitoDTO);
            if (errorRelaciones is not null)
            {
                return BadRequest(errorRelaciones);
            }

            // Todo remito nuevo nace Emitido: el stock del destino recien se acredita al
            // recepcionarlo con POST {id}/recepcion. El externo no descuenta origen; el
            // interno descuenta el origen al emitirse y acredita el destino al recibirse.
            bool esExterno = tipoCanonico == "Externo";

            Remito remito = new Remito
            {
                Tipo = tipoCanonico,
                EmpresaOrigen = esExterno ? remitoDTO.EmpresaOrigen?.Trim() : null,
                Numero = remitoDTO.Numero,
                FechaEmision = remitoDTO.FechaEmision,
                FechaRecepcion = null,
                Estado = "Emitido",
                EmpresaId = remitoDTO.EmpresaId,
                UsuarioId = remitoDTO.UsuarioId,
                CotizacionId = remitoDTO.CotizacionId,
                DepositoOrigenId = esExterno ? null : remitoDTO.DepositoOrigenId,
                DepositoDestinoId = remitoDTO.DepositoDestinoId
            };

            var items = remitoDTO.Items.Select(i => new ItemRemito
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad,
                Unidad = i.Unidad.Trim()
            }).ToList();

            try
            {
                await repositorio.RegistrarMovimiento(remito, items);
            }
            catch (StockInsuficienteException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(remito.Id);
        }

        /// <summary>
        /// Recepciona una transferencia emitida: acredita el stock del deposito destino y
        /// pasa el remito a Estado Recibido. La recepcion es total (no hay recepcion
        /// parcial de cantidades); si llega mercaderia dañada o de menos, el destino abre
        /// una Necesidad de vuelta al remitente en vez de ajustar este remito.
        /// </summary>
        [HttpPost("{id:int}/recepcion")]
        public async Task<ActionResult<bool>> Recepcion(int id, RecepcionRemitoDTO recepcionDTO)
        {
            var remito = await repositorio.SelectById(id);
            if (remito is null)
            {
                return NotFound($"No se encontró el remito de id: {id}");
            }

            if (remito.Tipo != "Externo" && remito.Tipo != "Interno")
            {
                return BadRequest("Solo los remitos de tipo Externo e Interno tienen recepción.");
            }

            if (remito.Estado != "Emitido")
            {
                return BadRequest("El remito ya fue recibido.");
            }

            await repositorio.RegistrarRecepcion(id, recepcionDTO.FechaRecepcion);

            return Ok(true);
        }

        // No hay PUT ni DELETE: un remito ya registrado no se modifica, porque eso
        // dejaria el stock impactado sin corresponder con lo que dice el remito.

        /// <summary>
        /// Valida el Tipo (sin distinguir mayusculas/minusculas) y que traiga los depositos
        /// que ese tipo necesita. Devuelve el Tipo canonico por out param.
        /// </summary>
        private static string? ValidarTipoYDepositos(RemitoDTO dto, out string tipoCanonico)
        {
            if (string.Equals(dto.Tipo, "Externo", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Externo";
                if (string.IsNullOrWhiteSpace(dto.EmpresaOrigen))
                {
                    return "El remito de tipo Externo requiere la empresa de origen.";
                }

                if (!dto.DepositoDestinoId.HasValue)
                {
                    return "El remito de tipo Externo requiere un depósito destino.";
                }
            }
            else if (string.Equals(dto.Tipo, "Interno", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Interno";
                if (!dto.DepositoOrigenId.HasValue || !dto.DepositoDestinoId.HasValue)
                {
                    return "El remito de tipo Interno requiere depósito origen y depósito destino.";
                }

                if (dto.DepositoOrigenId == dto.DepositoDestinoId)
                {
                    return "El depósito origen y el depósito destino deben ser distintos.";
                }
            }
            else
            {
                tipoCanonico = string.Empty;
                return $"Tipo de remito inválido: {dto.Tipo}. Debe ser Externo o Interno.";
            }

            return null;
        }

        /// <summary>
        /// Las FK exigen que empresa, usuario, cotizacion (si hay), depositos y productos
        /// de los items existan. Devuelve null cuando esta todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(RemitoDTO dto)
        {
            if (!await empresaRepositorio.Existe(dto.EmpresaId))
            {
                return $"No existe la empresa de id: {dto.EmpresaId}";
            }

            if (!await usuarioRepositorio.Existe(dto.UsuarioId))
            {
                return $"No existe el usuario de id: {dto.UsuarioId}";
            }

            if (dto.CotizacionId.HasValue && !await cotizacionRepositorio.Existe(dto.CotizacionId.Value))
            {
                return $"No existe la cotización de id: {dto.CotizacionId}";
            }

            if (dto.DepositoOrigenId.HasValue && !await depositoRepositorio.Existe(dto.DepositoOrigenId.Value))
            {
                return $"No existe el depósito origen de id: {dto.DepositoOrigenId}";
            }

            if (dto.DepositoDestinoId.HasValue && !await depositoRepositorio.Existe(dto.DepositoDestinoId.Value))
            {
                return $"No existe el depósito destino de id: {dto.DepositoDestinoId}";
            }

            foreach (var item in dto.Items)
            {
                if (!await productoRepositorio.Existe(item.ProductoId))
                {
                    return $"No existe el producto de id: {item.ProductoId}";
                }
            }

            return null;
        }
    }
}
