using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Server.Seguridad;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/nota-pedido")]
    public class NotaPedidoController : Controller
    {
        private readonly INotaPedidoRepositorio repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Usuario> usuarioRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;
        private readonly INecesidadRepositorio necesidadRepositorio;
        private readonly IRepositorio<ItemNota> itemNotaRepositorio;
        private readonly IRepositorio<Cotizacion> cotizacionRepositorio;
        private readonly IPermisosUsuario permisos;

        public NotaPedidoController(
            INotaPedidoRepositorio repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio,
            IRepositorio<Producto> productoRepositorio,
            INecesidadRepositorio necesidadRepositorio,
            IRepositorio<ItemNota> itemNotaRepositorio,
            IRepositorio<Cotizacion> cotizacionRepositorio,
            IPermisosUsuario permisos)
        {
            this.permisos = permisos;
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
            this.productoRepositorio = productoRepositorio;
            this.necesidadRepositorio = necesidadRepositorio;
            this.itemNotaRepositorio = itemNotaRepositorio;
            this.cotizacionRepositorio = cotizacionRepositorio;
        }

        /// <summary>
        /// Marketplace: notas Publicadas, no vencidas y de otras empresas, con sus ítems,
        /// la empresa compradora y si la empresa del usuario ya las cotizó. Solo Ventas o
        /// SuperAdmin; es la única vista de notas ajenas y no cambia los demás permisos.
        /// </summary>
        [HttpGet("marketplace")]
        public async Task<ActionResult<List<OportunidadMarketplaceDTO>>> GetMarketplace(
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var hoy = DateTime.Today;
            var notas = (await repositorio.Select())
                .Where(n => EsPublicada(n.Estado)
                    && n.FechaVencimiento.Date >= hoy
                    && n.EmpresaId != permisos.EmpresaId)
                .OrderBy(n => n.FechaVencimiento)
                .ThenBy(n => n.Id)
                .ToList();

            if (notas.Count == 0)
            {
                return Ok(new List<OportunidadMarketplaceDTO>());
            }

            var idsNotas = notas.Select(n => n.Id).ToHashSet();
            var itemsPorNota = (await itemNotaRepositorio.Select())
                .Where(i => idsNotas.Contains(i.NotaPedidoId))
                .GroupBy(i => i.NotaPedidoId)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Id).ToList());
            var nombresProductos = (await productoRepositorio.Select())
                .ToDictionary(p => p.Id, p => p.Nombre);
            var empresas = (await empresaRepositorio.Select())
                .ToDictionary(e => e.Id, e => e.RazonSocial);
            var cotizadas = (await cotizacionRepositorio.Select())
                .Where(c => c.EmpresaProveedoraId == permisos.EmpresaId)
                .Select(c => c.NotaPedidoId)
                .ToHashSet();

            var oportunidades = notas.Select(n => new OportunidadMarketplaceDTO
            {
                Id = n.Id,
                EmpresaId = n.EmpresaId,
                EmpresaRazonSocial = empresas.GetValueOrDefault(n.EmpresaId, string.Empty),
                Observaciones = n.Observaciones,
                FechaVencimiento = n.FechaVencimiento,
                Items = itemsPorNota.GetValueOrDefault(n.Id, []).Select(i => new ItemNotaDTO
                {
                    Id = i.Id,
                    ProductoId = i.ProductoId,
                    ProductoNombre = nombresProductos.GetValueOrDefault(i.ProductoId, string.Empty),
                    CantidadSolicitada = i.CantidadSolicitada
                }).ToList(),
                Cotizada = cotizadas.Contains(n.Id)
            }).ToList();

            return Ok(oportunidades);
        }

        [HttpGet]
        public async Task<ActionResult<List<NotaPedido>>> Get(
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

            var necesidades = await NecesidadesPorId();
            var notas = (await repositorio.Select())
                .Where(n => PuedeVer(n, necesidades))
                .ToList();

            // depositoId: solo las notas cuya necesidad de origen toca ese depósito.
            if (depositoId.HasValue)
            {
                notas = notas
                    .Where(n => n.NecesidadId.HasValue
                        && necesidades.TryGetValue(n.NecesidadId.Value, out var necesidad)
                        && (necesidad.DepositoSolicitanteId == depositoId.Value || necesidad.DepositoDestinoId == depositoId.Value))
                    .ToList();
            }

            return Ok(notas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NotaPedidoDTO>> GetById(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            var (notaPedido, items) = await repositorio.ObtenerDetalle(id);
            if (notaPedido is null)
            {
                return NotFound($"No se encontró la nota de pedido de id: {id}");
            }

            if (!PuedeVer(notaPedido, await NecesidadesPorId()))
            {
                return StatusCode(StatusCodes.Status403Forbidden, "No tenés acceso a esa nota de pedido.");
            }

            // Los nombres de producto se resuelven en una sola consulta para no
            // depender de navegaciones que el repositorio no carga.
            var nombresProductos = (await productoRepositorio.Select())
                .ToDictionary(p => p.Id, p => p.Nombre);

            NotaPedidoDTO dto = new NotaPedidoDTO
            {
                Estado = notaPedido.Estado,
                FechaEmision = notaPedido.FechaEmision,
                FechaVencimiento = notaPedido.FechaVencimiento,
                Observaciones = notaPedido.Observaciones,
                EmpresaId = notaPedido.EmpresaId,
                UsuarioId = notaPedido.UsuarioId,
                AprobadoPorId = notaPedido.AprobadoPorId,
                NecesidadId = notaPedido.NecesidadId ?? 0,
                Items = items.Select(i => new ItemNotaDTO
                {
                    Id = i.Id,
                    ProductoId = i.ProductoId,
                    ProductoNombre = nombresProductos.GetValueOrDefault(i.ProductoId, string.Empty),
                    CantidadSolicitada = i.CantidadSolicitada
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(NotaPedidoDTO notaPedidoDTO)
        {
            if (notaPedidoDTO.Items is null || notaPedidoDTO.Items.Count == 0)
            {
                return BadRequest("La nota de pedido debe tener al menos un ítem.");
            }

            // Una nota solo pasa a Publicada por POST api/nota-pedido/{id}/publicar.
            if (EsPublicada(notaPedidoDTO.Estado))
            {
                return BadRequest("Una nota de pedido no se puede crear como Publicada: usá la acción publicar.");
            }

            var error = await ValidarRelaciones(notaPedidoDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            // La necesidad ya se validó en ValidarRelaciones: existe.
            var necesidad = await necesidadRepositorio.SelectById(notaPedidoDTO.NecesidadId);
            decimal stockDisponible = await necesidadRepositorio.ObtenerStockDisponible(necesidad!.DepositoDestinoId, necesidad.ProductoId);
            if (stockDisponible >= necesidad.CantidadRequerida)
            {
                return BadRequest("El depósito destino ya tiene stock suficiente para esta necesidad.");
            }

            NotaPedido notaPedido = new NotaPedido
            {
                Estado = notaPedidoDTO.Estado,
                FechaEmision = notaPedidoDTO.FechaEmision,
                FechaVencimiento = notaPedidoDTO.FechaVencimiento,
                Observaciones = notaPedidoDTO.Observaciones,
                EmpresaId = notaPedidoDTO.EmpresaId,
                UsuarioId = notaPedidoDTO.UsuarioId,
                AprobadoPorId = notaPedidoDTO.AprobadoPorId,
                NecesidadId = notaPedidoDTO.NecesidadId
            };

            var items = notaPedidoDTO.Items.Select(i => new ItemNota
            {
                ProductoId = i.ProductoId,
                CantidadSolicitada = i.CantidadSolicitada
            }).ToList();

            await repositorio.InsertarConItems(notaPedido, items);

            return Ok(notaPedido.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, NotaPedidoDTO notaPedidoDTO)
        {
            var notaPedido = await repositorio.SelectById(id);
            if (notaPedido is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            // El PUT no puede publicar una nota; guardar una ya Publicada con el mismo estado sí se permite.
            if (EsPublicada(notaPedidoDTO.Estado) && !EsPublicada(notaPedido.Estado))
            {
                return BadRequest("El estado no se puede cambiar a Publicada con PUT: usá la acción publicar.");
            }

            if (!await empresaRepositorio.Existe(notaPedidoDTO.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {notaPedidoDTO.EmpresaId}");
            }

            if (!await usuarioRepositorio.Existe(notaPedidoDTO.UsuarioId))
            {
                return BadRequest($"No existe el usuario de id: {notaPedidoDTO.UsuarioId}");
            }

            if (notaPedidoDTO.AprobadoPorId.HasValue && !await usuarioRepositorio.Existe(notaPedidoDTO.AprobadoPorId.Value))
            {
                return BadRequest($"No existe el usuario aprobador de id: {notaPedidoDTO.AprobadoPorId}");
            }

            // Solo se actualiza el encabezado y el estado: los items no se tocan aca.
            notaPedido.Estado = notaPedidoDTO.Estado;
            notaPedido.FechaEmision = notaPedidoDTO.FechaEmision;
            notaPedido.FechaVencimiento = notaPedidoDTO.FechaVencimiento;
            notaPedido.Observaciones = notaPedidoDTO.Observaciones;
            notaPedido.EmpresaId = notaPedidoDTO.EmpresaId;
            notaPedido.UsuarioId = notaPedidoDTO.UsuarioId;
            notaPedido.AprobadoPorId = notaPedidoDTO.AprobadoPorId;

            var resultado = await repositorio.Update(notaPedido);
            return Ok(resultado);
        }

        /// <summary>
        /// Pasa la nota de Borrador a Publicada. Solo Ventas o SuperAdmin de la empresa de la nota.
        /// </summary>
        [HttpPost("{id:int}/publicar")]
        public async Task<ActionResult<bool>> Publicar(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var notaPedido = await repositorio.SelectById(id);
            if (notaPedido is null)
            {
                return NotFound($"No se encontró la nota de pedido de id: {id}");
            }

            acceso = permisos.ExigirEmpresa(notaPedido.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (!string.Equals(notaPedido.Estado, EstadosNotaPedido.Borrador, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest($"Solo se puede publicar una nota en estado {EstadosNotaPedido.Borrador}; esta está en {notaPedido.Estado}.");
            }

            notaPedido.Estado = EstadosNotaPedido.Publicada;
            await repositorio.Update(notaPedido);

            return Ok(true);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe la nota de pedido con id: {id}");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }

        private static bool EsPublicada(string? estado)
        {
            return string.Equals(estado, EstadosNotaPedido.Publicada, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<Dictionary<int, Necesidad>> NecesidadesPorId()
        {
            return (await necesidadRepositorio.Select()).ToDictionary(n => n.Id);
        }

        /// <summary>
        /// Ventas y SuperAdmin ven las notas de su empresa. El encargado ve las notas cuya necesidad
        /// de origen tiene como solicitante o destino un depósito que puede ver; sin necesidad, no.
        /// </summary>
        private bool PuedeVer(NotaPedido nota, Dictionary<int, Necesidad> necesidades)
        {
            if (permisos.PuedeGestionar)
            {
                return permisos.PuedeVerEmpresa(nota.EmpresaId);
            }

            return nota.NecesidadId.HasValue
                && necesidades.TryGetValue(nota.NecesidadId.Value, out var necesidad)
                && (permisos.PuedeVerDeposito(necesidad.DepositoSolicitanteId) || permisos.PuedeVerDeposito(necesidad.DepositoDestinoId));
        }

        /// <summary>
        /// Las FK exigen que empresa, usuario creador, aprobador (si hay), necesidad y
        /// productos de los items existan. Devuelve null cuando esta todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(NotaPedidoDTO dto)
        {
            if (!await empresaRepositorio.Existe(dto.EmpresaId))
            {
                return $"No existe la empresa de id: {dto.EmpresaId}";
            }

            if (!await usuarioRepositorio.Existe(dto.UsuarioId))
            {
                return $"No existe el usuario de id: {dto.UsuarioId}";
            }

            if (dto.AprobadoPorId.HasValue && !await usuarioRepositorio.Existe(dto.AprobadoPorId.Value))
            {
                return $"No existe el usuario aprobador de id: {dto.AprobadoPorId}";
            }

            if (!await necesidadRepositorio.Existe(dto.NecesidadId))
            {
                return $"No existe la necesidad de id: {dto.NecesidadId}";
            }

            // Una necesidad solo puede tener una nota de pedido: la vinculación es 1 a 1.
            var notasDeLaNecesidad = await repositorio.ObtenerPorNecesidad(dto.NecesidadId);
            if (notasDeLaNecesidad.Count > 0)
            {
                return $"La necesidad de id: {dto.NecesidadId} ya tiene la nota de pedido N.º {notasDeLaNecesidad[0].Id}.";
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
