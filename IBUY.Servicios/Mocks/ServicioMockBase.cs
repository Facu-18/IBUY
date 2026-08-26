using IBUY.Servicios.Interfaces;
using IBUY.Shared.DTO;

namespace IBUY.Servicios.Mocks
{
    /// <summary>
    /// Implementacion comun del CRUD en memoria. Cada servicio concreto solo indica
    /// sobre que coleccion de la <see cref="BaseDatosMock"/> trabaja.
    /// </summary>
    public abstract class ServicioMockBase<T> : IServicioMock<T> where T : DtoBase
    {
        protected readonly BaseDatosMock bd;

        protected ServicioMockBase(BaseDatosMock bd)
        {
            this.bd = bd;
        }

        protected abstract List<T> Coleccion { get; }

        public async Task<List<T>> ObtenerTodosAsync()
        {
            await SimularDemora();
            return [.. Coleccion];
        }

        public async Task<T?> ObtenerPorIdAsync(int id)
        {
            await SimularDemora();
            return Coleccion.FirstOrDefault(elemento => elemento.Id == id);
        }

        public async Task<T> CrearAsync(T dto)
        {
            await SimularDemora();
            dto.Id = bd.SiguienteId(Coleccion);
            Coleccion.Add(dto);
            return dto;
        }

        public async Task<bool> ActualizarAsync(int id, T dto)
        {
            await SimularDemora();

            var indice = Coleccion.FindIndex(elemento => elemento.Id == id);
            if (indice < 0)
            {
                return false;
            }

            dto.Id = id;
            Coleccion[indice] = dto;
            return true;
        }

        protected static Task SimularDemora() => Task.Delay(BaseDatosMock.Retardo);
    }
}
