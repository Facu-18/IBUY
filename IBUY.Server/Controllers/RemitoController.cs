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
                Tipo = remito.Tipo,
                Numero = remito.Numero,
                FechaEmision = remito.FechaEmision,
                FechaRecepcion = remito.FechaRecepcion,
                EmpresaId = remito.EmpresaId,
                UsuarioId = remito.UsuarioId,
                CotizacionId = remito.CotizacionId,
                DepositoOrigenId = remito.DepositoOrigenId,
                DepositoDestinoId = remito.DepositoDestinoId,
                Items = items.Select(i => new ItemRemitoDTO
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad
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

            Remito remito = new Remito
            {
                Tipo = tipoCanonico,
                Numero = remitoDTO.Numero,
                FechaEmision = remitoDTO.FechaEmision,
                FechaRecepcion = remitoDTO.FechaRecepcion,
                EmpresaId = remitoDTO.EmpresaId,
                UsuarioId = remitoDTO.UsuarioId,
                CotizacionId = remitoDTO.CotizacionId,
                DepositoOrigenId = remitoDTO.DepositoOrigenId,
                DepositoDestinoId = remitoDTO.DepositoDestinoId
            };

            var items = remitoDTO.Items.Select(i => new ItemRemito
            {
                ProductoId = i.ProductoId,
                Cantidad = i.Cantidad
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

        // No hay PUT ni DELETE: un remito ya registrado no se modifica, porque eso
        // dejaria el stock impactado sin corresponder con lo que dice el remito.

        /// <summary>
        /// Valida el Tipo (sin distinguir mayusculas/minusculas) y que traiga los depositos
        /// que ese tipo necesita. Devuelve el Tipo canonico por out param.
        /// </summary>
        private static string? ValidarTipoYDepositos(RemitoDTO dto, out string tipoCanonico)
        {
            if (string.Equals(dto.Tipo, "Entrada", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Entrada";
                if (!dto.DepositoDestinoId.HasValue)
                {
                    return "El remito de tipo Entrada requiere un depósito destino.";
                }
            }
            else if (string.Equals(dto.Tipo, "Salida", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Salida";
                if (!dto.DepositoOrigenId.HasValue)
                {
                    return "El remito de tipo Salida requiere un depósito origen.";
                }
            }
            else if (string.Equals(dto.Tipo, "Transferencia", StringComparison.OrdinalIgnoreCase))
            {
                tipoCanonico = "Transferencia";
                if (!dto.DepositoOrigenId.HasValue || !dto.DepositoDestinoId.HasValue)
                {
                    return "El remito de tipo Transferencia requiere depósito origen y depósito destino.";
                }

                if (dto.DepositoOrigenId == dto.DepositoDestinoId)
                {
                    return "El depósito origen y el depósito destino deben ser distintos.";
                }
            }
            else
            {
                tipoCanonico = string.Empty;
                return $"Tipo de remito inválido: {dto.Tipo}. Debe ser Entrada, Salida o Transferencia.";
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
