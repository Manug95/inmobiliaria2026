using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IPropietarioRepository : IRepository<Propietario, int>
{
    public Task<IList<Propietario>> ListarPropietarios(
        string? nomApe = null,
        string? orderBy = null,
        string? order = "ASC",
        int? offset = null,
        int? limit = null
    );
    public Task<int> ContarPropietarios();
}