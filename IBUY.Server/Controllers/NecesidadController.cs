using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/necesidad")]
    public class NecesidadController : Controller
    {
        /// <summary>Cantidad propuesta cuando un producto no tiene stock ni mínimo definido.</summary>
        private const decimal CantidadSugeridaPorDefecto = 1m;

        private readonly INecesidadRepositorio repositorio;
        private readonly IRepositorio<Deposito> depositoRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Usuario> usuarioRepositorio;

        public NecesidadController(
            INecesidadRepositorio repositorio,
            IRepositorio<Deposito> depositoRepositorio,
            IRepositorio<Producto> productoRepositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio)
        {
            this.repositorio = repositorio;
            this.depositoRepositorio = depositoRepositorio;
            this.productoRepositorio = productoRepositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
        }

        /// <summary>
        /// Necesidades de los depósitos de una empresa, con nombre de producto y depósitos
        /// ya resueltos, más los filtros opcionales de estado y depósito.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<NecesidadDTO>>> Get(
            [FromQuery] int? empresaId,
            [FromQuery] string? estado,
            [FromQuery] int? depositoId)
        {
            var necesidades = empresaId.HasValue
                ? await repositorio.ObtenerDeEmpresa(empresaId.Value, estado, depositoId)
                : await repositorio.Select();

            if (!empresaId.HasValue)
            {
                if (depositoId.HasValue)
                {
                    necesidades = necesidades
                        .Where(n => n.DepositoDestinoId == depositoId.Value || n.DepositoSolicitanteId == depositoId.Value)
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(estado))
                {
                    necesidades = necesidades.Where(n => n.Estado == estado).ToList();
                }
            }

            return Ok(await CompletarNecesidades(necesidades));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NecesidadDTO>> GetById(int id)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No se encontró la necesidad de id: {id}");
            }

            var dtos = await CompletarNecesidades([necesidad]);
            return Ok(dtos[0]);
        }

        /// <summary>
        /// Solo informa el stock disponible en el depósito destino: no crea ni modifica nada.
        /// </summary>
        [HttpGet("{id:int}/revision")]
        public async Task<ActionResult<RevisionNecesidadDTO>> GetRevision(int id)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No se encontró la necesidad de id: {id}");
            }

            decimal stockDisponible = await repositorio.ObtenerStockDisponible(necesidad.DepositoDestinoId, necesidad.ProductoId);
            decimal faltante = Math.Max(0, necesidad.CantidadRequerida - stockDisponible);

            RevisionNecesidadDTO dto = new RevisionNecesidadDTO
            {
                NecesidadId = necesidad.Id,
                ProductoId = necesidad.ProductoId,
                DepositoDestinoId = necesidad.DepositoDestinoId,
                CantidadRequerida = necesidad.CantidadRequerida,
                StockDisponible = stockDisponible,
                Alcanza = stockDisponible >= necesidad.CantidadRequerida,
                Faltante = faltante
            };

            return Ok(dto);
        }

        /// <summary>
        /// Productos con stock crítico (sin existencias) en un depósito, con el depósito
        /// desde el que conviene pedirlos. Es solo una propuesta: no crea necesidades.
        /// </summary>
        [HttpGet("sugerencias")]
        public async Task<ActionResult<List<SugerenciaNecesidadDTO>>> GetSugerencias([FromQuery] int depositoId)
        {
            var deposito = await depositoRepositorio.SelectById(depositoId);
            if (deposito is null)
            {
                return NotFound($"No existe el depósito de id: {depositoId}");
            }

            var depositosDeEmpresa = await repositorio.ObtenerDepositosDeEmpresa(deposito.EmpresaId);
            var solicitante = depositosDeEmpresa
                .Where(d => d.Id != depositoId)
                .OrderByDescending(d => d.Activo)
                .ThenBy(d => d.Id)
                .FirstOrDefault();

            var productos = (await productoRepositorio.Select())
                .Where(p => p.EmpresaId == deposito.EmpresaId)
                .ToDictionary(p => p.Id);

            var pendientes = (await repositorio.Select())
                .Where(n => n.Estado == EstadosNecesidad.Pendiente)
                .Select(n => (n.DepositoSolicitanteId, n.DepositoDestinoId, n.ProductoId))
                .ToHashSet();

            var stocks = await repositorio.ObtenerStockDeDeposito(depositoId);
            var sugerencias = stocks
                .Where(s => s.CantidadActual <= 0 && productos.ContainsKey(s.ProductoId))
                .Select(s => new SugerenciaNecesidadDTO
                {
                    ProductoId = s.ProductoId,
                    ProductoNombre = productos[s.ProductoId].Nombre,
                    Categoria = productos[s.ProductoId].Categoria,
                    DepositoDestinoId = depositoId,
                    DepositoDestinoNombre = deposito.Nombre,
                    DepositoSolicitanteId = solicitante?.Id ?? 0,
                    DepositoSolicitanteNombre = solicitante?.Nombre ?? string.Empty,
                    CantidadActual = s.CantidadActual,
                    CantidadSugerida = CantidadSugeridaPorDefecto,
                    YaTienePendiente = solicitante is not null
                        && pendientes.Contains((solicitante.Id, depositoId, s.ProductoId))
                })
                .OrderBy(s => s.ProductoNombre)
                .ToList();

            return Ok(sugerencias);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post(NecesidadDTO necesidadDTO)
        {
            var error = await ValidarNecesidad(necesidadDTO, null);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Necesidad necesidad = new Necesidad();
            AplicarCambios(necesidad, necesidadDTO);

            await repositorio.Insert(necesidad);

            return Ok(necesidad.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, NecesidadDTO necesidadDTO)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            var error = await ValidarNecesidad(necesidadDTO, id);
            if (error is not null)
            {
                return BadRequest(error);
            }

            AplicarCambios(necesidad, necesidadDTO);

            var resultado = await repositorio.Update(necesidad);
            return Ok(resultado);
        }

        /// <summary>
        /// Genera la nota de pedido de una necesidad. El ítem se completa solo con el
        /// producto y la cantidad requerida, y la necesidad queda vinculada en estado "En nota".
        /// </summary>
        [HttpPost("{id:int}/nota-pedido")]
        public async Task<ActionResult<int>> GenerarNotaPedido(int id, CrearNotaDesdeNecesidadDTO dto)
        {
            var necesidad = await repositorio.SelectById(id);
            if (necesidad is null)
            {
                return NotFound($"No se encontró la necesidad de id: {id}");
            }

            if (necesidad.Estado == EstadosNecesidad.EnNota)
            {
                return BadRequest("La necesidad ya tiene una nota de pedido generada.");
            }

            if (necesidad.Estado == EstadosNecesidad.Cubierta)
            {
                return BadRequest("La necesidad ya está cubierta: no hace falta una nota de pedido.");
            }

            var depositoDestino = await depositoRepositorio.SelectById(necesidad.DepositoDestinoId);
            if (depositoDestino is null)
            {
                return BadRequest($"No existe el depósito destino de id: {necesidad.DepositoDestinoId}");
            }

            if (!await empresaRepositorio.Existe(depositoDestino.EmpresaId))
            {
                return BadRequest($"No existe la empresa de id: {depositoDestino.EmpresaId}");
            }

            if (!await usuarioRepositorio.Existe(dto.UsuarioId))
            {
                return BadRequest($"No existe el usuario de id: {dto.UsuarioId}");
            }

            if (dto.FechaVencimiento.Date < DateTime.Today)
            {
                return BadRequest("La fecha de vencimiento no puede ser anterior a hoy.");
            }

            var notas = await repositorio.ObtenerNotasPorNecesidad();
            if (notas.ContainsKey(id))
            {
                return BadRequest($"La necesidad ya está vinculada a la nota de pedido N.º {notas[id]}.");
            }

            var notaPedido = new NotaPedido
            {
                Estado = EstadosNotaPedido.Borrador,
                FechaEmision = DateTime.Today,
                FechaVencimiento = dto.FechaVencimiento.Date,
                Observaciones = dto.Observaciones ?? string.Empty,
                EmpresaId = depositoDestino.EmpresaId,
                UsuarioId = dto.UsuarioId,
                AprobadoPorId = null,
                NecesidadId = necesidad.Id
            };

            var creada = await repositorio.GenerarNotaPedido(necesidad, notaPedido);

            return Ok(creada.Id);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            if (!await repositorio.Existe(id))
            {
                return NotFound($"No existe la necesidad con id: {id}");
            }

            var notas = await repositorio.ObtenerNotasPorNecesidad();
            if (notas.ContainsKey(id))
            {
                return BadRequest($"La necesidad está vinculada a la nota de pedido N.º {notas[id]} y no se puede eliminar.");
            }

            await repositorio.Delete(id);

            return Ok(true);
        }

        /// <summary>
        /// Copia los campos editables del DTO a la entidad, normalizando el estado.
        /// </summary>
        private static void AplicarCambios(Necesidad necesidad, NecesidadDTO dto)
        {
            necesidad.CantidadRequerida = dto.CantidadRequerida;
            necesidad.FechaRequerida = dto.FechaRequerida.Date;
            necesidad.Estado = string.IsNullOrWhiteSpace(dto.Estado) ? EstadosNecesidad.Pendiente : dto.Estado;
            necesidad.DepositoSolicitanteId = dto.DepositoSolicitanteId;
            necesidad.DepositoDestinoId = dto.DepositoDestinoId;
            necesidad.ProductoId = dto.ProductoId;
        }

        /// <summary>
        /// Valida cantidad, estado, relaciones y que no se duplique una necesidad pendiente.
        /// Devuelve null cuando está todo bien.
        /// </summary>
        private async Task<string?> ValidarNecesidad(NecesidadDTO dto, int? excluirId)
        {
            if (dto.CantidadRequerida <= 0)
            {
                return "La cantidad requerida debe ser mayor a cero.";
            }

            var estado = string.IsNullOrWhiteSpace(dto.Estado) ? EstadosNecesidad.Pendiente : dto.Estado;
            if (!EstadosNecesidad.EsValido(estado))
            {
                return $"El estado debe ser uno de: {string.Join(", ", EstadosNecesidad.Todos)}.";
            }

            var error = await ValidarRelaciones(dto);
            if (error is not null)
            {
                return error;
            }

            if (estado == EstadosNecesidad.Pendiente
                && await repositorio.ExistePendienteDuplicada(
                    dto.DepositoSolicitanteId, dto.DepositoDestinoId, dto.ProductoId, excluirId))
            {
                return "Ya existe una necesidad pendiente para ese producto y esos depósitos.";
            }

            return null;
        }

        /// <summary>
        /// Las FK exigen que el deposito y el producto existan. Sin este control,
        /// SQL Server rechaza el INSERT y el error sale como un 500 en vez de un
        /// mensaje util. Devuelve null cuando esta todo bien.
        /// </summary>
        private async Task<string?> ValidarRelaciones(NecesidadDTO necesidadDTO)
        {
            if (!await depositoRepositorio.Existe(necesidadDTO.DepositoSolicitanteId))
            {
                return $"No existe el depósito solicitante de id: {necesidadDTO.DepositoSolicitanteId}";
            }

            if (!await depositoRepositorio.Existe(necesidadDTO.DepositoDestinoId))
            {
                return $"No existe el depósito destino de id: {necesidadDTO.DepositoDestinoId}";
            }

            if (necesidadDTO.DepositoSolicitanteId == necesidadDTO.DepositoDestinoId)
            {
                return "El depósito solicitante y el depósito destino deben ser distintos.";
            }

            if (!await productoRepositorio.Existe(necesidadDTO.ProductoId))
            {
                return $"No existe el producto de id: {necesidadDTO.ProductoId}";
            }

            return null;
        }

        /// <summary>
        /// Resuelve nombre de producto, nombres de depósitos, empresa, nota vinculada y
        /// stock disponible, para que el listado y el detalle no dependan de navegaciones.
        /// </summary>
        private async Task<List<NecesidadDTO>> CompletarNecesidades(List<Necesidad> necesidades)
        {
            if (necesidades.Count == 0)
            {
                return [];
            }

            var productos = (await productoRepositorio.Select()).ToDictionary(p => p.Id);
            var depositos = (await depositoRepositorio.Select()).ToDictionary(d => d.Id);
            var notas = await repositorio.ObtenerNotasPorNecesidad();

            // El stock se resuelve por depósito destino y producto, una sola vez por combinación.
            var stockPorDestino = new Dictionary<(int DepositoId, int ProductoId), decimal>();
            foreach (var necesidad in necesidades)
            {
                var clave = (necesidad.DepositoDestinoId, necesidad.ProductoId);
                if (!stockPorDestino.ContainsKey(clave))
                {
                    stockPorDestino[clave] = await repositorio.ObtenerStockDisponible(necesidad.DepositoDestinoId, necesidad.ProductoId);
                }
            }

            return necesidades
                .Select(n => new NecesidadDTO
                {
                    Id = n.Id,
                    CantidadRequerida = n.CantidadRequerida,
                    FechaRequerida = n.FechaRequerida,
                    Estado = n.Estado,
                    DepositoSolicitanteId = n.DepositoSolicitanteId,
                    DepositoDestinoId = n.DepositoDestinoId,
                    ProductoId = n.ProductoId,
                    ProductoNombre = productos.TryGetValue(n.ProductoId, out var producto) ? producto.Nombre : string.Empty,
                    ProductoCategoria = productos.TryGetValue(n.ProductoId, out var productoCat) ? productoCat.Categoria : string.Empty,
                    DepositoSolicitanteNombre = depositos.TryGetValue(n.DepositoSolicitanteId, out var solicitante) ? solicitante.Nombre : string.Empty,
                    DepositoDestinoNombre = depositos.TryGetValue(n.DepositoDestinoId, out var destino) ? destino.Nombre : string.Empty,
                    EmpresaId = depositos.TryGetValue(n.DepositoDestinoId, out var empresaDestino) ? empresaDestino.EmpresaId : 0,
                    NotaPedidoId = notas.TryGetValue(n.Id, out var nota) ? nota : 0,
                    StockDisponible = stockPorDestino[(n.DepositoDestinoId, n.ProductoId)]
                })
                .OrderBy(n => n.Estado == EstadosNecesidad.Pendiente ? 0 : 1)
                .ThenBy(n => n.FechaRequerida)
                .ThenBy(n => n.Id)
                .ToList();
        }
    }
}
