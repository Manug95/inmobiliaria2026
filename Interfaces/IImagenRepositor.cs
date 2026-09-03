using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IImagenRepository : IRepository<Imagen, long>
{
    public Task<List<Imagen>> ListarPorInmuebleAsync(int inmuebleId, int limit, int offset);
}