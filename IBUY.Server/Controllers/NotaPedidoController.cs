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
        public async Task<ActionResult<List<NotaPedidoResumenDTO>>> Get(
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

            var idsNotas = notas.Select(n => n.Id).ToHashSet();
            var itemsPorNota = (await itemNotaRepositorio.Select())
                .Where(i => idsNotas.Contains(i.NotaPedidoId))
                .GroupBy(i => i.NotaPedidoId)
                .ToDictionary(g => g.Key, g => g.Count());
            var cotizacionesPorNota = (await cotizacionRepositorio.Select())
                .Where(c => idsNotas.Contains(c.NotaPedidoId))
                .GroupBy(c => c.NotaPedidoId)
                .ToDictionary(g => g.Key, g => g.Count());

            var resumenes = notas
                .OrderByDescending(n => n.Id)
                .Select(n => new NotaPedidoResumenDTO
                {
                    Id = n.Id,
                    Estado = n.Estado,
                    FechaEmision = n.FechaEmision,
                    FechaVencimiento = n.FechaVencimiento,
                    Observaciones = n.Observaciones,
                    EmpresaId = n.EmpresaId,
                    NecesidadId = n.NecesidadId,
                    AprobadoPorId = n.AprobadoPorId,
                    CantidadItems = itemsPorNota.GetValueOrDefault(n.Id),
                    CantidadCotizaciones = cotizacionesPorNota.GetValueOrDefault(n.Id)
                })
                .ToList();

            return Ok(resumenes);
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

            // Los nombres se resuelven en una sola consulta para no depender de
            // navegaciones que el repositorio no carga.
            var nombresProductos = (await productoRepositorio.Select())
                .ToDictionary(p => p.Id, p => p.Nombre);
            var empresa = await empresaRepositorio.SelectById(notaPedido.EmpresaId);
            var usuario = await usuarioRepositorio.SelectById(notaPedido.UsuarioId);
            var aprobador = notaPedido.AprobadoPorId.HasValue
                ? await usuarioRepositorio.SelectById(notaPedido.AprobadoPorId.Value)
                : null;
            var cantidadCotizaciones = (await cotizacionRepositorio.Select())
                .Count(c => c.NotaPedidoId == notaPedido.Id);

            NotaPedidoDTO dto = new NotaPedidoDTO
            {
                Id = notaPedido.Id,
                Estado = notaPedido.Estado,
                FechaEmision = notaPedido.FechaEmision,
                FechaVencimiento = notaPedido.FechaVencimiento,
                Observaciones = notaPedido.Observaciones,
                EmpresaId = notaPedido.EmpresaId,
                EmpresaRazonSocial = empresa?.RazonSocial ?? string.Empty,
                UsuarioId = notaPedido.UsuarioId,
                UsuarioNombre = usuario?.Nombre ?? string.Empty,
                AprobadoPorId = notaPedido.AprobadoPorId,
                AprobadoPorNombre = aprobador?.Nombre ?? string.Empty,
                NecesidadId = notaPedido.NecesidadId,
                CantidadCotizaciones = cantidadCotizaciones,
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

        /// <summary>
        /// Crea una nota en Borrador. La fecha de emisión la pone el servidor (hoy) y el
        /// vencimiento debe ser posterior. La necesidad de origen es opcional.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Post(
            NotaPedidoDTO notaPedidoDTO,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId)
                ?? permisos.ExigirGestion()
                ?? permisos.ExigirEmpresa(notaPedidoDTO.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            // Una nota nueva siempre nace en Borrador: aprobar y publicar son acciones aparte.
            if (!EsBorrador(notaPedidoDTO.Estado))
            {
                return BadRequest($"Una nota de pedido nueva siempre se crea en estado {EstadosNotaPedido.Borrador}.");
            }

            var error = await ValidarRelaciones(notaPedidoDTO, esEdicion: false);
            if (error is not null)
            {
                return BadRequest(error);
            }

            var hoy = DateTime.Today;
            if (notaPedidoDTO.FechaVencimiento.Date <= hoy)
            {
                return BadRequest("La fecha de vencimiento debe ser posterior a la fecha de emisión.");
            }

            if (notaPedidoDTO.NecesidadId.HasValue)
            {
                // La necesidad ya se validó en ValidarRelaciones: existe.
                var necesidad = await necesidadRepositorio.SelectById(notaPedidoDTO.NecesidadId.Value);
                decimal stockDisponible = await necesidadRepositorio.ObtenerStockDisponible(necesidad!.DepositoDestinoId, necesidad.ProductoId);
                if (stockDisponible >= necesidad.CantidadRequerida)
                {
                    return BadRequest("El depósito destino ya tiene stock suficiente para esta necesidad.");
                }
            }

            NotaPedido notaPedido = new NotaPedido
            {
                Estado = EstadosNotaPedido.Borrador,
                FechaEmision = hoy,
                FechaVencimiento = notaPedidoDTO.FechaVencimiento.Date,
                Observaciones = notaPedidoDTO.Observaciones,
                EmpresaId = notaPedidoDTO.EmpresaId,
                UsuarioId = notaPedidoDTO.UsuarioId,
                AprobadoPorId = null,
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

        /// <summary>
        /// Edita el encabezado y los ítems. No se permite si la nota tiene cotizaciones.
        /// Editar una nota Aprobada la devuelve a Borrador para que se vuelva a aprobar.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(
            int id,
            NotaPedidoDTO notaPedidoDTO,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var notaPedido = await repositorio.SelectById(id);
            if (notaPedido is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            acceso = permisos.ExigirEmpresa(notaPedido.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (await repositorio.TieneCotizaciones(id))
            {
                return BadRequest("No se puede editar una nota de pedido que ya tiene cotizaciones.");
            }

            if (!EsEditable(notaPedido.Estado))
            {
                return BadRequest($"No se puede editar una nota en estado {notaPedido.Estado}.");
            }

            // El estado solo cambia con las acciones aprobar, publicar o cancelar.
            if (!string.Equals(notaPedidoDTO.Estado, notaPedido.Estado, StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El estado no se puede cambiar desde la edición: usá las acciones aprobar, publicar o cancelar.");
            }

            if (notaPedidoDTO.EmpresaId != notaPedido.EmpresaId)
            {
                return BadRequest("No se puede cambiar la empresa de una nota de pedido.");
            }

            if (notaPedidoDTO.NecesidadId.HasValue && notaPedidoDTO.NecesidadId != notaPedido.NecesidadId)
            {
                return BadRequest("No se puede cambiar la necesidad de origen de una nota de pedido.");
            }

            var error = await ValidarRelaciones(notaPedidoDTO, esEdicion: true);
            if (error is not null)
            {
                return BadRequest(error);
            }

            if (notaPedidoDTO.FechaVencimiento.Date <= notaPedido.FechaEmision.Date)
            {
                return BadRequest("La fecha de vencimiento debe ser posterior a la fecha de emisión.");
            }

            if (EsPublicada(notaPedido.Estado) && notaPedidoDTO.FechaVencimiento.Date < DateTime.Today)
            {
                return BadRequest("Una nota publicada no puede quedar con fecha de vencimiento pasada.");
            }

            notaPedido.FechaVencimiento = notaPedidoDTO.FechaVencimiento.Date;
            notaPedido.Observaciones = notaPedidoDTO.Observaciones;
            notaPedido.UsuarioId = notaPedidoDTO.UsuarioId;

            if (EsAprobada(notaPedido.Estado))
            {
                // Un cambio en los datos invalida la aprobación anterior.
                notaPedido.Estado = EstadosNotaPedido.Borrador;
                notaPedido.AprobadoPorId = null;
            }

            var items = notaPedidoDTO.Items.Select(i => new ItemNota
            {
                ProductoId = i.ProductoId,
                CantidadSolicitada = i.CantidadSolicitada
            }).ToList();

            await repositorio.ActualizarConItems(notaPedido, items);

            return Ok(true);
        }

        /// <summary>
        /// Pasa la nota de Borrador a Aprobada y registra quién la aprobó.
        /// Solo Ventas o SuperAdmin de la empresa de la nota.
        /// </summary>
        [HttpPost("{id:int}/aprobar")]
        public async Task<ActionResult<bool>> Aprobar(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
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

            if (!EsBorrador(notaPedido.Estado))
            {
                return BadRequest($"Solo se puede aprobar una nota en estado {EstadosNotaPedido.Borrador}; esta está en {notaPedido.Estado}.");
            }

            if (notaPedido.FechaVencimiento.Date < DateTime.Today)
            {
                return BadRequest("La nota está vencida: actualizá el vencimiento antes de aprobarla.");
            }

            notaPedido.Estado = EstadosNotaPedido.Aprobada;
            notaPedido.AprobadoPorId = permisos.UsuarioId;
            await repositorio.Update(notaPedido);

            return Ok(true);
        }

        /// <summary>
        /// Pasa la nota de Aprobada a Publicada. Solo Ventas o SuperAdmin de la empresa.
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

            if (!EsAprobada(notaPedido.Estado))
            {
                return BadRequest($"Solo se puede publicar una nota en estado {EstadosNotaPedido.Aprobada}; esta está en {notaPedido.Estado}.");
            }

            if (notaPedido.FechaVencimiento.Date < DateTime.Today)
            {
                return BadRequest("La nota está vencida y no se puede publicar.");
            }

            var (_, items) = await repositorio.ObtenerDetalle(id);
            if (items.Count == 0)
            {
                return BadRequest("La nota debe tener al menos un ítem para publicarse.");
            }

            notaPedido.Estado = EstadosNotaPedido.Publicada;
            await repositorio.Update(notaPedido);

            return Ok(true);
        }

        /// <summary>
        /// Cancela la nota. No se puede cancelar una nota Cerrada o ya Cancelada.
        /// </summary>
        [HttpPost("{id:int}/cancelar")]
        public async Task<ActionResult<bool>> Cancelar(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
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

            if (EsCerrada(notaPedido.Estado) || EsCancelada(notaPedido.Estado))
            {
                return BadRequest($"No se puede cancelar una nota en estado {notaPedido.Estado}.");
            }

            notaPedido.Estado = EstadosNotaPedido.Cancelada;
            await repositorio.Update(notaPedido);

            return Ok(true);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var notaPedido = await repositorio.SelectById(id);
            if (notaPedido is null)
            {
                return NotFound($"No existe la nota de pedido con id: {id}");
            }

            acceso = permisos.ExigirEmpresa(notaPedido.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (await repositorio.TieneCotizaciones(id))
            {
                return BadRequest("No se puede eliminar una nota de pedido que ya tiene cotizaciones.");
            }

            if (!EsBorrador(notaPedido.Estado) && !EsCancelada(notaPedido.Estado))
            {
                return BadRequest($"Solo se pueden eliminar notas en estado {EstadosNotaPedido.Borrador} o {EstadosNotaPedido.Cancelada}.");
            }

            await repositorio.EliminarConItems(id);

            return Ok(true);
        }

        private static bool EsBorrador(string? estado) => EsEstado(estado, EstadosNotaPedido.Borrador);

        private static bool EsAprobada(string? estado) => EsEstado(estado, EstadosNotaPedido.Aprobada);

        private static bool EsPublicada(string? estado) => EsEstado(estado, EstadosNotaPedido.Publicada);

        private static bool EsCerrada(string? estado) => EsEstado(estado, EstadosNotaPedido.Cerrada);

        private static bool EsCancelada(string? estado) => EsEstado(estado, EstadosNotaPedido.Cancelada);

        private static bool EsEditable(string? estado) => EsBorrador(estado) || EsAprobada(estado) || EsPublicada(estado);

        private static bool EsEstado(string? estado, string esperado) =>
            string.Equals(estado, esperado, StringComparison.OrdinalIgnoreCase);

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
        /// Valida empresa, usuario creador, necesidad (si hay), productos y cantidades.
        /// Devuelve null cuando está todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(NotaPedidoDTO dto, bool esEdicion)
        {
            if (dto.Items is null || dto.Items.Count == 0)
            {
                return "La nota de pedido debe tener al menos un ítem.";
            }

            if (!await empresaRepositorio.Existe(dto.EmpresaId))
            {
                return $"No existe la empresa de id: {dto.EmpresaId}";
            }

            var usuario = await usuarioRepositorio.SelectById(dto.UsuarioId);
            if (usuario is null)
            {
                return $"No existe el usuario de id: {dto.UsuarioId}";
            }

            if (usuario.EmpresaId != dto.EmpresaId)
            {
                return "El usuario que crea la nota no pertenece a la empresa de la nota.";
            }

            if (dto.NecesidadId.HasValue)
            {
                if (!await necesidadRepositorio.Existe(dto.NecesidadId.Value))
                {
                    return $"No existe la necesidad de id: {dto.NecesidadId.Value}";
                }

                if (!esEdicion)
                {
                    // Una necesidad solo puede tener una nota de pedido: la vinculación es 1 a 1.
                    var notasDeLaNecesidad = await repositorio.ObtenerPorNecesidad(dto.NecesidadId.Value);
                    if (notasDeLaNecesidad.Count > 0)
                    {
                        return $"La necesidad de id: {dto.NecesidadId.Value} ya tiene la nota de pedido N.º {notasDeLaNecesidad[0].Id}.";
                    }
                }
            }

            var productos = new HashSet<int>();
            var productosPorId = (await productoRepositorio.Select()).ToDictionary(p => p.Id);
            foreach (var item in dto.Items)
            {
                if (item.ProductoId <= 0)
                {
                    return "Debe seleccionar un producto en cada ítem.";
                }

                if (!productosPorId.TryGetValue(item.ProductoId, out var producto))
                {
                    return $"No existe el producto de id: {item.ProductoId}";
                }

                if (producto.EmpresaId != dto.EmpresaId)
                {
                    return $"El producto {producto.Nombre} no pertenece a la empresa de la nota.";
                }

                if (item.CantidadSolicitada <= 0)
                {
                    return "Las cantidades solicitadas deben ser mayores a cero.";
                }

                if (!productos.Add(item.ProductoId))
                {
                    return "No se puede repetir el mismo producto en dos ítems.";
                }
            }

            return null;
        }
    }
}
