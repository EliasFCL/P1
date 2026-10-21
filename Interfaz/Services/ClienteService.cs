//Servicios: https://learn.microsoft.com/ca-es/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.0#7
//Convertir JSON a un objeto: https://learn.microsoft.com/id-id/dotnet/csharp/tutorials/console-webapiclient#1

using Models;
using System.Net.Http.Json;

namespace Proyecto1_EliasCorderoLeon.Services
{
    public interface IClienteService
    {
        Task<List<Cliente>> ListarAsync();
        Task<Cliente?> BuscarPorNumIdAsync(string numId);
        Task<Cliente?> BuscarPorIdAsync(int id);
        Task<(bool ok, List<string> errores)> CrearAsync(Cliente cliente);
        Task<(bool ok, List<string> errores)> ActualizarAsync(int id, Cliente cliente);
        Task<bool> EliminarAsync(int id);
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

        public async Task<bool> EliminarAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/clientes/{id}");
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