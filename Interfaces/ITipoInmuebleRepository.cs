using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface ITipoInmuebleRepository : IRepository<TipoInmueble, int>
{
    public Task<int> ContarTiposInmueble();
}