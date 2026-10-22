//Crear Async: https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0
//Comprender como adaptar LeerErrores: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/use-dom

using Microsoft.AspNetCore.Mvc;
using Models;

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
        private static async Task<List<string>> LeerErrores(HttpResponseMessage resp)
        {
            var body = await resp.Content.ReadAsStringAsync();
            try
            {
                using var doc = System.Text.Json.JsonDocument.Parse(body);
                foreach (var key in new[] { "errores", "errors" })
                {
                    if (doc.RootElement.TryGetProperty(key, out var el))
                    {
                        if (el.ValueKind == System.Text.Json.JsonValueKind.Array)
                            return el.EnumerateArray().Select(x => x.GetString() ?? "").ToList();
                        if (el.ValueKind == System.Text.Json.JsonValueKind.Object)
                            return el.EnumerateObject()
                                     .SelectMany(p => p.Value.EnumerateArray().Select(v => v.GetString() ?? ""))
                                     .ToList();
                    }
                }
            }
            catch { }
            return new List<string> { $"HTTP {(int)resp.StatusCode}: {body}" };
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
    }
}