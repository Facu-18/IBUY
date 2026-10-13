using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace IBUY.Servicios.ServiciosHTTP
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient http;

        public HttpService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<HttpResp<T?>> GetAsync<T>(string url)
        {
            try
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return await CrearErrorAsync<T>(response);
                }

                var dato = await response.Content.ReadFromJsonAsync<T>();

                return new HttpResp<T?>(dato, false, response, null);

            }
            catch (Exception e)
            {
                return new HttpResp<T?>(default, true, null, e.Message);
            }

        }

        public async Task<HttpResp<TResp?>> PostAsync<T, TResp>(string url, T DTO)
        {
            try
            {
                var JsonEnviar = JsonSerializer.Serialize(DTO);
                var cuerpo = new StringContent(JsonEnviar, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(url, cuerpo);
                if (!response.IsSuccessStatusCode)
                {
                    return await CrearErrorAsync<TResp>(response);
                }
                else
                {
                    var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResp?>> PutAsync<T, TResp>(string url, T DTO)
        {
            try
            {
                var JsonEnviar = JsonSerializer.Serialize(DTO);
                var cuerpo = new StringContent(JsonEnviar,
                                               Encoding.UTF8,
                                               "application/json");

                var response = await http.PutAsync(url, cuerpo);
                if (!response.IsSuccessStatusCode)
                {
                    return await CrearErrorAsync<TResp>(response);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var dato = DesSerializar<TResp>(content);

                    //var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
                //+ " " + e.InnerException?.Message 
                //+ " " + e.StackTrace);
            }
        }

        public async Task<HttpResp<TResp?>> DeleteAsync<TResp>(string url)
        {
            try
            {
                var response = await http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return await CrearErrorAsync<TResp>(response);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var dato = DesSerializar<TResp>(content);
                    //var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
            }
        }

        /// <summary>
        /// Arma la respuesta de error leyendo el cuerpo: los controladores devuelven el
        /// mensaje como texto plano (BadRequest("...")) o como ProblemDetails en JSON.
        /// </summary>
        private async Task<HttpResp<T?>> CrearErrorAsync<T>(HttpResponseMessage response)
        {
            return new HttpResp<T?>(default, true, response, await LeerMensajeError(response));
        }

        private static async Task<string?> LeerMensajeError(HttpResponseMessage response)
        {
            try
            {
                var contenido = (await response.Content.ReadAsStringAsync()).Trim();
                if (contenido.Length == 0)
                {
                    return null;
                }

                if (contenido.StartsWith('{'))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(contenido);

                        var erroresValidacion = ExtraerErroresValidacion(doc.RootElement);
                        if (!string.IsNullOrWhiteSpace(erroresValidacion))
                        {
                            return erroresValidacion;
                        }

                        foreach (var clave in new[] { "detail", "title", "message" })
                        {
                            if (doc.RootElement.TryGetProperty(clave, out var valor) &&
                                valor.ValueKind == JsonValueKind.String &&
                                !string.IsNullOrWhiteSpace(valor.GetString()))
                            {
                                return valor.GetString();
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // No era JSON valido: se devuelve tal cual mas abajo.
                    }
                }

                return contenido.Trim('"');
            }
            catch
            {
                return null;
            }
        }

        private static string? ExtraerErroresValidacion(JsonElement raiz)
        {
            if (raiz.ValueKind != JsonValueKind.Object ||
                !raiz.TryGetProperty("errors", out var errores) ||
                errores.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var mensajes = new List<string>();
            foreach (var propiedad in errores.EnumerateObject())
            {
                if (propiedad.Value.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var item in propiedad.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String &&
                        !string.IsNullOrWhiteSpace(item.GetString()) &&
                        !mensajes.Contains(item.GetString()!))
                    {
                        mensajes.Add(item.GetString()!);
                    }
                }
            }

            return mensajes.Count == 0 ? null : string.Join("; ", mensajes);
        }
        private T? DesSerializar<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content,
                                                new JsonSerializerOptions
                                                {
                                                    PropertyNameCaseInsensitive = true
                                                });
        }
    }
}
