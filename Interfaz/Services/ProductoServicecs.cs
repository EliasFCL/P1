//Consulta a la API (VerificarCombinacion): https://nugetprodusnc-northcentralus-01.regional.azure-api.net/packages/Myth.Testing/4.3.0-preview.2#3

using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ListarAsync();
        Task<Producto?> BuscarPorIdAsync(Guid id);
        Task<(bool ok, List<string> errores)> CrearAsync(Producto producto);
        Task<(bool ok, List<string> errores)> ActualizarAsync(Guid id, Producto producto);
        Task<(bool ok, List<string> errores)> EliminarAsync(Guid id);
        Task<Producto?> BuscarAsync(string busqueda);
        Task<(bool existe, double? precio)> VerificarCombinacionAsync(Producto producto);
    }

    public class ProductoService : IProductoService
    {
        private readonly HttpClient _http;

        public ProductoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Producto>> ListarAsync()
        {
            return await _http.GetFromJsonAsync<List<Producto>>("api/productos") ?? new();
        }

        public async Task<Producto?> BuscarPorIdAsync(Guid id)
        {
            var resp = await _http.GetAsync($"api/productos/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Producto>();
        }

        public async Task<(bool ok, List<string> errores)> CrearAsync(Producto producto)
        {
            var resp = await _http.PostAsJsonAsync("api/productos", producto);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(Guid id, Producto producto)
        {
            var resp = await _http.PutAsJsonAsync($"api/productos/{id}", producto);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> EliminarAsync(Guid id)
        {
            var resp = await _http.DeleteAsync($"api/productos/{id}");
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        private static async Task<List<string>> LeerErrores(HttpResponseMessage response)
        {
            if (response.Content is null)
            {
                return new List<string> { response.ReasonPhrase ?? "Request fallido" };
            }

            var payload = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(payload))
            {
                return new List<string> { "Respuesta vacía del servidor" };
            }

            try
            {
                using var document = JsonDocument.Parse(payload);
                var root = document.RootElement;

                if (root.TryGetProperty("errores", out var erroresArray) && erroresArray.ValueKind == JsonValueKind.Array)
                {
                    var errores = new List<string>();
                    foreach (var item in erroresArray.EnumerateArray())
                    {
                        errores.Add(item.GetString() ?? "Errores desconocidos");
                    }
                    return errores;
                }

                if (root.TryGetProperty("errors", out var errorsObject) && errorsObject.ValueKind == JsonValueKind.Object)
                {
                    var errores = new List<string>();
                    foreach (var prop in errorsObject.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var item in prop.Value.EnumerateArray())
                            {
                                errores.Add(item.GetString() ?? "Error desconocido");
                            }
                        }
                    }
                    return errores;
                }
            }
            catch (JsonException)
            {
            }

            return new List<string> { payload };
        }


        private class ErrorPayload
        {
            public List<string>? errores { get; set; }
        }

        public async Task<Producto?> BuscarAsync(string busqueda)
        {
            var resp = await _http.GetAsync($"api/productos/buscar/{Uri.EscapeDataString(busqueda)}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Producto>();
        }

        public async Task<(bool existe, double? precio)> VerificarCombinacionAsync(Producto producto)
        {
            var resp = await _http.PostAsJsonAsync("api/productos/verificarCombinacion", new
            {
                unidadMedida = producto.unidadMedida,
                categoria = producto.categoria
            });
            if (!resp.IsSuccessStatusCode) return (false, null);
            var result = await resp.Content.ReadFromJsonAsync<CombinacionResult>();
            return (result?.existe ?? false, result?.precio);
        }

        private class CombinacionResult
        {
            public bool existe { get; set; }
            public double? precio { get; set; }
        }
    }
}