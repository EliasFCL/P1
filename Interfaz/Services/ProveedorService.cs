using Models;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IProveedorService
    {
        Task<List<Proveedor>> ListarAsync();
        Task<Proveedor?> BuscarPorNumIdAsync(string numId);
        Task<Proveedor?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Proveedor proveedor);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Proveedor proveedor);
        Task<bool> EliminarAsync(int id);
        Task<Proveedor?> BuscarAsync(string busqueda);
        Task<List<string>> ListarNombresAsync();
    }

    public class ProveedorService : IProveedorService
    {

        private readonly HttpClient _http;

        public ProveedorService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Proveedor>> ListarAsync()
        {
            return await _http.GetFromJsonAsync<List<Proveedor>>("api/proveedores") ?? new();
        }

        public async Task<Proveedor?> BuscarPorNumIdAsync(string numId)
        {
            var resp = await _http.GetAsync($"api/proveedores/porNumId/{numId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Proveedor>();
        }

        public async Task<Proveedor?> BuscarPorIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/proveedores/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Proveedor>();
        }

        public async Task<(bool ok, List<string> errores)> CrearAsync(Proveedor proveedor)
        {
            proveedor.apellido1 = "ABC";
            proveedor.apellido2 = "ABC";
            proveedor.fechaNacimiento = new DateTime(2004, 3, 25);
            var resp = await _http.PostAsJsonAsync("api/proveedores", proveedor);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Proveedor proveedor)
        {
            proveedor.apellido1 = "ABC";
            proveedor.apellido2 = "ABC";
            proveedor.fechaNacimiento = new DateTime(2004, 3, 25);
            var resp = await _http.PutAsJsonAsync($"api/proveedores/{id}", proveedor);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/proveedores/{id}");
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

        public async Task<Proveedor?> BuscarAsync(string busqueda)
        {
            var resp = await _http.GetAsync($"api/proveedores/buscar/{Uri.EscapeDataString(busqueda)}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Proveedor>();
        }

        public async Task<List<string>> ListarNombresAsync()
        {
            return await _http.GetFromJsonAsync<List<string>>("api/proveedores/nombres") ?? new();
        }
    }
}
