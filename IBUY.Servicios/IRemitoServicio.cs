using IBUY.Servicios.ServiciosHTTP;
using IBUY.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;


namespace IBUY.Servicios
{
    public interface IRemitoServicio
    {
        Task<HttpResp<List<ItemRemitoDTO>>> ObtenerTodosAsync();

        Task<HttpResp<ItemRemitoDTO>> ObtenerPorIdAsync(int id);

        Task<HttpResp<ItemRemitoDTO>> CrearAsync(ItemRemitoDTO remito);

        Task<HttpResp<ItemRemitoDTO>> RegistrarRecepcionAsync(
            int id,
            RecepcionRemitoDTO recepcion);
    }
}