//Consulta a la API: https://nugetprodusnc-northcentralus-01.regional.azure-api.net/packages/Myth.Testing/4.3.0-preview.2#3

using Models;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ListarAsync();
        Task<Producto?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Producto producto);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Producto producto);
        Task<bool> EliminarAsync(int id);
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

        public async Task<bool> EliminarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/productos/{id}");
            return resp.IsSuccessStatusCode;
        }

        private static async Task<List<string>> LeerErrores(HttpResponseMessage resp)
        {
            try
            {
                var payload = await resp.Content.ReadFromJsonAsync<ErrorPayload>();
                return payload?.errores ?? new List<string> { "Error desconocido" };
            }
            catch
            {
                return new List<string> { $"Error {(int)resp.StatusCode}" };
            }
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
