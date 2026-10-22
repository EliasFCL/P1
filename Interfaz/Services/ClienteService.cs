//Servicios: https://learn.microsoft.com/ca-es/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0#7
//Crear Async: https://learn.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-10.0
//Convertir JSON a un objeto: https://learn.microsoft.com/id-id/dotnet/csharp/tutorials/console-webapiclient#1
//Leer errores: https://github.com/honua-io/honua-server/pull/257/changes/7433aa52cfc052fba78733f4abf4d6665ad700d8#3

using Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IClienteService
    {
        Task<List<Cliente>> ListarAsync();
        Task<Cliente?> BuscarPorNumIdAsync(string numId);
        Task<Cliente?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Cliente cliente);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Cliente cliente);
        Task<(bool ok, List<string> errores)> EliminarAsync(int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly HttpClient _http;

        public ClienteService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Cliente>> ListarAsync()
        {
            return await _http.GetFromJsonAsync<List<Cliente>>("api/clientes") ?? new();
        }

        public async Task<Cliente?> BuscarPorNumIdAsync(string numId)
        {
            var resp = await _http.GetAsync($"api/clientes/porNumId/{numId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Cliente>();
        }

        public async Task<Cliente?> BuscarPorIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/clientes/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Cliente>();
        }

        public async Task<(bool ok, List<string> errores)> CrearAsync(Cliente cliente)
        {
            var resp = await _http.PostAsJsonAsync("api/clientes", cliente);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Cliente cliente)
        {
            var resp = await _http.PutAsJsonAsync($"api/clientes/{id}", cliente);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> EliminarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/clientes/{id}");
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
    }
}