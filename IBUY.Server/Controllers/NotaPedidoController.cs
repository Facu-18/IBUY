using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/notapedido")]
    public class NotaPedidoController : Controller
    {
        private readonly INotaPedidoRepositorio repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Usuario> usuarioRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;
        private readonly INecesidadRepositorio necesidadRepositorio;

        public NotaPedidoController(
            INotaPedidoRepositorio repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio,
            IRepositorio<Producto> productoRepositorio,
            INecesidadRepositorio necesidadRepositorio)
        {
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
            this.productoRepositorio = productoRepositorio;
            this.necesidadRepositorio = necesidadRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotaPedido>>> Get()
        {
            var notas = await repositorio.Select();
            return Ok(notas);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<NotaPedidoDTO>> GetById(int id)
        {
            var (notaPedido, items) = await repositorio.ObtenerDetalle(id);
            if (notaPedido is null)
            {
                return NotFound($"No se encontró la nota de pedido de id: {id}");
            }

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
                    ProductoId = i.ProductoId,
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
