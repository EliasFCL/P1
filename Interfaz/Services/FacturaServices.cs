//Comprender como adaptar LeerErrores: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom
// y https://blog.devgenius.io/speech-transcription-using-azure-speech-services-and-c-9811708ae209

using Microsoft.AspNetCore.Mvc;
using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IFacturaService
    {
        Task<List<Factura>> ListarAsync();
        Task<(bool ok, List<string> errores, Factura? factura)> CrearAsync(Factura factura);
        Task<Factura?> BuscarPorNumeroAsync(string numero);
        Task<List<Factura>> BuscarAsync(string criterio);
        Task<bool> EliminarAsync(string numero);
        Task<(bool ok, List<string> errores)> ActualizarAsync(string numero, Factura factura);
        Task<(bool ok, List<string> errores)> AnularAsync(string numero);
    }
    public class FacturaService : IFacturaService
    {
        private readonly HttpClient _http;

        public FacturaService(HttpClient http)
        {
            _http = http;
        }

        public async Task<(bool ok, List<string> errores, Factura? factura)> CrearAsync(Factura factura)
        {
            var resp = await _http.PostAsJsonAsync("api/facturas", factura);
            if (resp.IsSuccessStatusCode)
            {
                var creada = await resp.Content.ReadFromJsonAsync<Factura>();
                return (true, new(), creada);
            }
            return (false, await LeerErrores(resp), null);
        }

        public async Task<List<Factura>> ListarAsync()
        {
            var resp = await _http.GetAsync("api/facturas");
            if (!resp.IsSuccessStatusCode) return new List<Factura>();
            return await resp.Content.ReadFromJsonAsync<List<Factura>>();
        }

        public async Task<Factura?> BuscarPorNumeroAsync(string numero)
        {
            var resp = await _http.GetAsync($"api/facturas/{numero}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Factura>();
        }

        //Mostrar el error real 
        private static async Task<List<string>> LeerErrores(HttpResponseMessage response)
        {
            if (response.Content is null)
            {
                return new List<string> { response.ReasonPhrase ?? "Request fallido" };
            }

            var payload = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(payload))
            {
                return new List<string> { "Empty response from server" };
            }

            try
            {
                using var document = JsonDocument.Parse(payload);
                var root = document.RootElement;

                //Formato: {"errores": ["Mensaje 1", "Mensaje 2"]}
                if (root.TryGetProperty("errores", out var erroresArray) && erroresArray.ValueKind == JsonValueKind.Array)
                {
                    var errores = new List<string>();
                    foreach (var item in erroresArray.EnumerateArray())
                    {
                        errores.Add(item.GetString() ?? "Unknown error");
                    }
                    return errores;
                }

                //Formato: {"errors": {"Campo": ["Mensaje"]}}
                if (root.TryGetProperty("errors", out var errorsObject) && errorsObject.ValueKind == JsonValueKind.Object)
                {
                    var errores = new List<string>();
                    foreach (var prop in errorsObject.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                errores.Add(item.GetString() ?? "Unknown error");
                            }
                        }
                    }
                    return errores;
                }
            }
            catch (JsonException)
            {
                //Ignoramos si el JSON está mal formado
            }

            //Fallback si no encontró las claves o falló el parseo
            return new List<string> { payload };
        }


        private class ErrorPayload
        {
            public List<string>? errores { get; set; }
        }

        public async Task<List<Factura>> BuscarAsync(string criterio)
        {
            var resp = await _http.GetAsync($"api/facturas/buscar/{Uri.EscapeDataString(criterio)}");
            if (!resp.IsSuccessStatusCode) return new List<Factura>();
            return await resp.Content.ReadFromJsonAsync<List<Factura>>() ?? new();
        }

        public async Task<bool> EliminarAsync(string numero)
        {
            var resp = await _http.DeleteAsync($"api/facturas/{numero}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(string numero, Factura factura)
        {
            var resp = await _http.PutAsJsonAsync($"api/facturas/{numero}", factura);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> AnularAsync(string numero)
        {
            var resp = await _http.PutAsync($"api/facturas/{numero}/anular", null);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }
    }
}