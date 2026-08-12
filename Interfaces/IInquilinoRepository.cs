using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IInquilinoRepository : IRepository<Inquilino, int>
{
    public Task<IList<Inquilino>> ListarInquilinos(
        string? nomApe = null,
        string? orderBy = null,
        string? order = "ASC",
        int? limit = null,
        int? offset = null
    );
    public Task<int> ContarInquilinos();
}