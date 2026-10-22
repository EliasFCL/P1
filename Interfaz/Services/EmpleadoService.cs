using Models;
using System.Text.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
        public interface IEmpleadoService
        {
            Task<List<Empleado>> ListarAsync();
            Task<Empleado?> BuscarPorNumIdAsync(string numId);
            Task<Empleado?> BuscarPorIdAsync(int id);
            Task<(bool ok, List<string> errores)> CrearAsync(Empleado empleado);
            Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Empleado empleado);
            Task<(bool ok, List<string> errores)> EliminarAsync(int id);
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

            public async Task<Empleado?> BuscarPorIdAsync(int id)
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

            public async Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Empleado empleado)
            {
                var resp = await _http.PutAsJsonAsync($"api/empleados/{id}", empleado);
                if (resp.IsSuccessStatusCode) return (true, new());
                return (false, await LeerErrores(resp));
            }

        public async Task<(bool ok, List<string> errores)> EliminarAsync(int id)
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