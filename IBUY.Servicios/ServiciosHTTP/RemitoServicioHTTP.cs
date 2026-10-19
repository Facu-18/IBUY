using IBUY.Servicios.ServiciosHTTP;
using IBUY.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace IBUY.Servicios.ServiciosHTTP


{
    public class RemitoServicioHttp : IRemitoServicio
    {
        private readonly IHttpService httpService;
        private const string Url = "api/remito";

        public RemitoServicioHttp(IHttpService httpService)
        {
            this.httpService = httpService;
        }

        public async Task<HttpResp<List<ItemRemitoDTO>>> ObtenerTodosAsync()
        {
            return await httpService.GetAsync<List<ItemRemitoDTO>>(Url);
        }

        public async Task<HttpResp<ItemRemitoDTO>> ObtenerPorIdAsync(int id)
        {
            return await httpService.GetAsync<ItemRemitoDTO>($"{Url}/{id}");
        }

        public async Task<HttpResp<ItemRemitoDTO>> CrearAsync(ItemRemitoDTO remito)
        {
            return await httpService.PostAsync<ItemRemitoDTO, ItemRemitoDTO>(Url, remito);
        }

        public async Task<HttpResp<ItemRemitoDTO>> RegistrarRecepcionAsync(
            int id,
            RecepcionRemitoDTO recepcion)
        {
            return await httpService.PostAsync<RecepcionRemitoDTO, ItemRemitoDTO>( $"{Url}/{id}/recepcion", recepcion);
        }
    }
}