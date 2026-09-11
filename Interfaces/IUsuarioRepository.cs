using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario, int>
    {
        public Task<Usuario?> ObtenerPorEmailAsync(string email);
        public Task<bool> ActualizarContraseñaAsync(int id, string password);
        public Task<int> ContarUsuariosAsync();
    }