using IBUY.BD.Datos.Entity;
using IBUY.Repository.Repositorios;
using IBUY.Server.Seguridad;
using IBUY.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace IBUY.Server.Controllers
{
    [ApiController]
    [Route("api/deposito")]
    public class DepositoController : Controller
    {
        
        private readonly IRepositorio<Deposito> repositorio;
        private readonly IRepositorio<Empresa> empresaRepositorio;
        private readonly IRepositorio<Usuario> usuarioRepositorio;
        private readonly IRepositorio<Stock> stockRepositorio;
        private readonly IRepositorio<Producto> productoRepositorio;
        private readonly IPermisosUsuario permisos;

        public DepositoController(
            IRepositorio<Deposito> repositorio,
            IRepositorio<Empresa> empresaRepositorio,
            IRepositorio<Usuario> usuarioRepositorio,
            IRepositorio<Stock> stockRepositorio,
            IRepositorio<Producto> productoRepositorio,
            IPermisosUsuario permisos)
        {
            this.permisos = permisos;
            this.repositorio = repositorio;
            this.empresaRepositorio = empresaRepositorio;
            this.usuarioRepositorio = usuarioRepositorio;
            this.stockRepositorio = stockRepositorio;
            this.productoRepositorio = productoRepositorio;
        }

        [HttpGet]
        public async Task<ActionResult<List<DepositoDTO>>> Get(
            [FromQuery] int? empresaId,
            [FromQuery] int? depositoId,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is null && empresaId.HasValue)
            {
                acceso = permisos.ExigirEmpresa(empresaId.Value);
            }

            if (acceso is null && depositoId.HasValue)
            {
                acceso = permisos.ExigirDeposito(depositoId.Value);
            }

            if (acceso is not null)
            {
                return acceso;
            }

            // Solo los depósitos que el usuario puede ver, más los filtros opcionales.
            var depositos = (await repositorio.Select())
                .Where(d => permisos.PuedeVerDeposito(d.Id))
                .ToList();

            if (empresaId.HasValue)
            {
                depositos = depositos.Where(d => d.EmpresaId == empresaId.Value).ToList();
            }

            if (depositoId.HasValue)
            {
                depositos = depositos.Where(d => d.Id == depositoId.Value).ToList();
            }

            return Ok(await CompletarDepositos(depositos));
        }

        /// <summary>
        /// Id y Nombre de los depósitos de la empresa del usuario, para los selectores de destino.
        /// Cualquier rol con acceso puede pedirlos; solo SuperAdmin puede indicar otra empresa.
        /// </summary>
        [HttpGet("opciones")]
        public async Task<ActionResult<List<DepositoOpcionDTO>>> GetOpciones(
            [FromQuery] int? empresaId,
            [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is null && empresaId.HasValue)
            {
                acceso = permisos.ExigirEmpresa(empresaId.Value);
            }

            if (acceso is not null)
            {
                return acceso;
            }

            int empresa = empresaId ?? permisos.EmpresaId;
            var opciones = (await repositorio.Select())
                .Where(d => d.EmpresaId == empresa)
                .OrderBy(d => d.Nombre)
                .Select(d => new DepositoOpcionDTO { Id = d.Id, Nombre = d.Nombre })
                .ToList();

            return Ok(opciones);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepositoDTO>> GetById(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            var deposito = await repositorio.SelectById(id);
            if (deposito is null)
            {
                return NotFound($"No se encontró el depósito de id: {id}");
            }

            acceso = permisos.ExigirDeposito(id);
            if (acceso is not null)
            {
                return acceso;
            }

            var dtos = await CompletarDepositos([deposito]);
            return Ok(dtos[0]);
        }

        /// <summary>
        /// Existencias del depósito: une stock con producto para poder mostrar nombre y categoría.
        /// </summary>
        [HttpGet("{id:int}/stock")]
        public async Task<ActionResult<List<StockDepositoDTO>>> GetStock(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId);
            if (acceso is not null)
            {
                return acceso;
            }

            if (!await repositorio.Existe(id))
            {
                return NotFound($"No se encontró el depósito de id: {id}");
            }

            acceso = permisos.ExigirDeposito(id);
            if (acceso is not null)
            {
                return acceso;
            }

            var stocks = (await stockRepositorio.Select())
                .Where(s => s.DepositoId == id)
                .ToList();

            var productos = await productoRepositorio.Select();
            var porId = productos.ToDictionary(p => p.Id);

            var respuesta = stocks
                .Where(s => porId.ContainsKey(s.ProductoId))
                .Select(s => new StockDepositoDTO
                {
                    StockId = s.Id,
                    ProductoId = s.ProductoId,
                    ProductoNombre = porId[s.ProductoId].Nombre,
                    Categoria = porId[s.ProductoId].Categoria,
                    CantidadActual = s.CantidadActual
                })
                .OrderBy(s => s.ProductoNombre)
                .ToList();

            return Ok(respuesta);
        }


        [HttpPost]
        public async Task<ActionResult<int>> Post(DepositoDTO depositoDTO, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion() ?? permisos.ExigirEmpresa(depositoDTO.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            var error = await ValidarRelaciones(depositoDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            Deposito deposito = new Deposito();
            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;
            deposito.UsuarioResponsableId = depositoDTO.UsuarioResponsableId;

            await repositorio.Insert(deposito);

            return Ok(deposito.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<bool>> Put(int id, DepositoDTO depositoDTO, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var deposito = await repositorio.SelectById(id);
            if (deposito is null)
            {
                return NotFound($"No existe el registro con id: {id}");
            }

            acceso = permisos.ExigirEmpresa(deposito.EmpresaId) ?? permisos.ExigirEmpresa(depositoDTO.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            var error = await ValidarRelaciones(depositoDTO);
            if (error is not null)
            {
                return BadRequest(error);
            }

            deposito.Nombre = depositoDTO.Nombre;
            deposito.Tipo = depositoDTO.Tipo;
            deposito.Direccion = depositoDTO.Direccion;
            deposito.Activo = depositoDTO.Activo;
            deposito.EmpresaId = depositoDTO.EmpresaId;
            deposito.UsuarioResponsableId = depositoDTO.UsuarioResponsableId;

            var resultado = await repositorio.Update(deposito);
            return Ok(resultado);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id, [FromHeader(Name = IPermisosUsuario.Encabezado)] string? usuarioId)
        {
            var acceso = await permisos.Autenticar(usuarioId) ?? permisos.ExigirGestion();
            if (acceso is not null)
            {
                return acceso;
            }

            var deposito = await repositorio.SelectById(id);
            if (deposito is null)
            {
                return NotFound($"No existe el depósito con id: {id}");
            }

            acceso = permisos.ExigirEmpresa(deposito.EmpresaId);
            if (acceso is not null)
            {
                return acceso;
            }

            await repositorio.Delete(id);

            return Ok(true);
        }

        /// <summary>
        /// Resuelve la razón social de la empresa y el nombre del responsable para cada depósito,
        /// para que el listado y el detalle no dependan de navegaciones que no se cargan.
        /// </summary>
        private async Task<List<DepositoDTO>> CompletarDepositos(List<Deposito> depositos)
        {
            if (depositos.Count == 0)
            {
                return [];
            }

            var empresas = await empresaRepositorio.Select();
            var usuarios = await usuarioRepositorio.Select();
            var empresasPorId = empresas.ToDictionary(e => e.Id);
            var usuariosPorId = usuarios.ToDictionary(u => u.Id);

            return depositos.Select(d => new DepositoDTO
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Tipo = d.Tipo,
                Direccion = d.Direccion,
                Activo = d.Activo,
                EmpresaId = d.EmpresaId,
                EmpresaRazonSocial = empresasPorId.TryGetValue(d.EmpresaId, out var empresa) ? empresa.RazonSocial : string.Empty,
                UsuarioResponsableId = d.UsuarioResponsableId,
                UsuarioResponsableNombre = d.UsuarioResponsableId.HasValue && usuariosPorId.TryGetValue(d.UsuarioResponsableId.Value, out var responsable)
                    ? responsable.Nombre
                    : string.Empty
            })
            .OrderBy(d => d.Nombre)
            .ToList();
        }

        /// <summary>
        /// Las FK exigen que la empresa y, si se informa, el usuario responsable existan.
        /// Sin este control, SQL Server rechaza el INSERT/UPDATE con un 500.
        /// </summary>
        private async Task<string?> ValidarRelaciones(DepositoDTO depositoDTO)
        {
            if (!await empresaRepositorio.Existe(depositoDTO.EmpresaId))
            {
                return $"No existe la empresa de id: {depositoDTO.EmpresaId}";
            }

            if (depositoDTO.UsuarioResponsableId.HasValue && !await usuarioRepositorio.Existe(depositoDTO.UsuarioResponsableId.Value))
            {
                return $"No existe el usuario de id: {depositoDTO.UsuarioResponsableId}";
            }

            return null;
        }
    }
}
