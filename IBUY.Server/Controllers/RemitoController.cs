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
        public async Task<ActionResult<List<RemitoListadoDTO>>> Get()
        {
            var remitos = await repositorio.ObtenerListado();

            var dtos = remitos.Select(r => new RemitoListadoDTO
            {
                Id = r.Id,
                Numero = r.Numero,
                // Interno: nombre del deposito origen; Externo: la empresa escrita a mano.
                Origen = r.Tipo == "Interno" ? r.DepositoOrigen?.Nombre ?? string.Empty : r.OrigenExterno ?? string.Empty,
                Destino = r.DepositoDestino?.Nombre ?? string.Empty,
                Tipo = r.Tipo,
                Fecha = r.FechaEmision,
                Estado = r.Estado,
                DepositoOrigenId = r.DepositoOrigenId,
                DepositoDestinoId = r.DepositoDestinoId
            }).ToList();

            return Ok(dtos);
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
                Tipo = remito.Tipo,
                Numero = remito.Numero,
                FechaEmision = remito.FechaEmision,
                FechaRecepcion = remito.FechaRecepcion,
                Estado = remito.Estado,
                OrigenExterno = remito.OrigenExterno,
                EmpresaId = remito.EmpresaId,
                UsuarioId = remito.UsuarioId,
                CotizacionId = remito.CotizacionId,
                DepositoOrigenId = remito.DepositoOrigenId,
                DepositoOrigenNombre = remito.DepositoOrigen?.Nombre,
                DepositoDestinoId = remito.DepositoDestinoId,
                DepositoDestinoNombre = remito.DepositoDestino?.Nombre,
                Items = items.Select(i => new ItemRemitoDTO
                {
                    ProductoId = i.ProductoId,
                    ProductoNombre = i.Producto?.Nombre,
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

            // Numero, Estado, FechaEmision y FechaRecepcion los decide el servidor, no el DTO:
            // todo remito nace Emitido, con la fecha de hoy y el numero R-0001 generado en
            // el repositorio. El stock del destino se suma recien al recibirlo.
            Remito remito = new Remito
            {
                Tipo = tipoCanonico,
                FechaEmision = DateTime.Today,
                FechaRecepcion = null,
                Estado = "Emitido",
                OrigenExterno = tipoCanonico == "Externo" ? remitoDTO.OrigenExterno!.Trim() : null,
                EmpresaId = remitoDTO.EmpresaId,
                UsuarioId = remitoDTO.UsuarioId,
                CotizacionId = remitoDTO.CotizacionId,
                DepositoOrigenId = remitoDTO.DepositoOrigenId,
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
        /// Recibe un remito emitido (Interno o Externo): acredita el stock del deposito
        /// destino y pasa el remito a Estado Recibido. La recepcion es total (no hay
        /// recepcion parcial de cantidades); si llega mercaderia dañada o de menos, el
        /// destino abre una Necesidad de vuelta al remitente en vez de ajustar este remito.
        /// </summary>
        [HttpPost("{id:int}/recepcion")]
        public async Task<ActionResult<bool>> Recepcion(int id, RecepcionRemitoDTO recepcionDTO)
        {
            try
            {
                var remito = await repositorio.RegistrarRecepcion(id, recepcionDTO.FechaRecepcion);
                if (remito is null)
                {
                    return NotFound($"No se encontró el remito de id: {id}");
                }
            }
            catch (RemitoYaRecibidoException)
            {
                return BadRequest("El remito ya fue recibido.");
            }

            return Ok(true);
        }

        // No hay PUT ni DELETE: un remito ya registrado no se modifica, porque eso
        // dejaria el stock impactado sin corresponder con lo que dice el remito.

        /// <summary>
        /// Valida el Tipo (sin distinguir mayusculas/minusculas) y que traiga lo que ese
        /// tipo necesita. Devuelve el Tipo canonico por out param.
        /// </summary>
        private static string? ValidarTipoYDepositos(RemitoDTO dto, out string tipoCanonico)
        {
            if (string.Equals(dto.Tipo, "Interno", StringComparison.OrdinalIgnoreCase))
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

                if (!string.IsNullOrWhiteSpace(dto.OrigenExterno))
                {
                    return "El remito de tipo Interno no admite origen externo.";
                }
            }
            else if (string.Equals(dto.Tipo, "Externo", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Externo";
                if (string.IsNullOrWhiteSpace(dto.OrigenExterno))
                {
                    return "El remito de tipo Externo requiere el origen externo.";
                }

                if (!dto.DepositoDestinoId.HasValue)
                {
                    return "El remito de tipo Externo requiere un depósito destino.";
                }

                if (dto.DepositoOrigenId.HasValue)
                {
                    return "El remito de tipo Externo no admite depósito origen.";
                }
            }
            else
            {
                tipoCanonico = string.Empty;
                return $"Tipo de remito inválido: {dto.Tipo}. Debe ser Interno o Externo.";
            }

            return null;
        }

        /// <summary>
        /// Las FK exigen que empresa, usuario, cotizacion (si hay), depositos y productos
        /// de los items existan; ademas los depositos deben ser de la empresa del remito.
        /// Devuelve null cuando esta todo bien.
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

            if (dto.DepositoOrigenId.HasValue)
            {
                var origen = await depositoRepositorio.SelectById(dto.DepositoOrigenId.Value);
                if (origen is null)
                {
                    return $"No existe el depósito origen de id: {dto.DepositoOrigenId}";
                }

                if (origen.EmpresaId != dto.EmpresaId)
                {
                    return "El depósito origen no pertenece a la empresa del remito.";
                }
            }

            if (dto.DepositoDestinoId.HasValue)
            {
                var destino = await depositoRepositorio.SelectById(dto.DepositoDestinoId.Value);
                if (destino is null)
                {
                    return $"No existe el depósito destino de id: {dto.DepositoDestinoId}";
                }

                if (destino.EmpresaId != dto.EmpresaId)
                {
                    return "El depósito destino no pertenece a la empresa del remito.";
                }
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
