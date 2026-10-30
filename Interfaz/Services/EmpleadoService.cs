using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IEmpleadoService
    {
        Task<List<Empleado>> ListarAsync();
        Task<Empleado?> BuscarPorNumIdAsync(string numId);
        Task<Empleado?> BuscarPorIdAsync(Guid id);
        Task<(bool ok, List<string> errores)> CrearAsync(Empleado empleado);
        Task<(bool ok, List<string> errores)> ActualizarAsync(Guid id, Empleado empleado);
        Task<(bool ok, List<string> errores)> EliminarAsync(Guid id);
    }

    public class EmpleadoService : IEmpleadoService
    {
        private readonly HttpClient _http;

        public EmpleadoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Empleado>> ListarAsync()
        {
            return await _http.GetFromJsonAsync<List<Empleado>>("api/empleados") ?? new();
        }

        public async Task<Empleado?> BuscarPorNumIdAsync(string numId)
        {
            var resp = await _http.GetAsync($"api/empleados/porNumId/{numId}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Empleado>();
        }

        public async Task<Empleado?> BuscarPorIdAsync(Guid id)
        {
            var resp = await _http.GetAsync($"api/empleados/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<Empleado>();
        }

        public async Task<(bool ok, List<string> errores)> CrearAsync(Empleado empleado)
        {
            var resp = await _http.PostAsJsonAsync("api/empleados", empleado);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> ActualizarAsync(Guid id, Empleado empleado)
        {
            var resp = await _http.PutAsJsonAsync($"api/empleados/{id}", empleado);
            if (resp.IsSuccessStatusCode) return (true, new());
            return (false, await LeerErrores(resp));
        }

        public async Task<(bool ok, List<string> errores)> EliminarAsync(Guid id)
        {
            var resp = await _http.DeleteAsync($"api/empleados/{id}");
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

                if (root.TryGetProperty("errores", out var erroresArray) && erroresArray.ValueKind == JsonValueKind.Array)
                {
                    var errores = new List<string>();
                    foreach (var item in erroresArray.EnumerateArray())
                    {
                        errores.Add(item.GetString() ?? "Unknown error");
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
                                errores.Add(item.GetString() ?? "Unknown error");
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
    }
}