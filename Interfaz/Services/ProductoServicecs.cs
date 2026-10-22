//Consulta a la API: https://nugetprodusnc-northcentralus-01.regional.azure-api.net/packages/Myth.Testing/4.3.0-preview.2#3

using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ListarAsync();
        Task<Producto?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Producto producto);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Producto producto);
        Task<(bool ok, List<string> errores)> EliminarAsync(int id);
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

        public async Task<Producto?> BuscarPorIdAsync(int id)
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

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Producto producto)
        {
            var resp = await _http.PutAsJsonAsync($"api/productos/{id}", producto);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> EliminarAsync(int id)
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

        public async Task<Producto?> BuscarAsync(string busqueda)
        {
            var resp = await _http.GetAsync($"api/productos/buscar/{Uri.EscapeDataString(busqueda)}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Producto>();
        }

        //Cosultarle a la API si hay una combinación, se obtiene el precio
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

        //Clase auxiliar para deserializar la respuesta de la API
        private class CombinacionResult
        {
            public bool existe { get; set; }
            public double? precio { get; set; }
        }
    }
}
