using Models;

namespace Proyecto1_EliasCorderoLeon.Services
{
        public interface IEmpleadoService
        {
            Task<List<Empleado>> ListarAsync();
            Task<Empleado?> BuscarPorNumIdAsync(string numId);
            Task<Empleado?> BuscarPorIdAsync(int id);
            Task<(bool ok, List<string> errores)> CrearAsync(Empleado empleado);
            Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Empleado empleado);
            Task<bool> EliminarAsync(int id);
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

            public async Task<bool> EliminarAsync(int id)
            {
                var resp = await _http.DeleteAsync($"api/empleados/{id}");
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
        }
    }