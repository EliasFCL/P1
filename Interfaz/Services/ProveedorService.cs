using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IProveedorService
    {
        Task<List<Proveedor>> ListarAsync();
        Task<Proveedor?> BuscarPorNumIdAsync(string numId);
        Task<Proveedor?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Proveedor proveedor);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Proveedor proveedor);
        Task<(bool ok, List<string> errores)> EliminarAsync(int id);
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

        public async Task<(bool ok, List<string> errores)> EliminarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/proveedores/{id}");
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
