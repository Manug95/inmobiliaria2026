using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IInmuebleRepository : IRepository<Inmueble, int>
{
    public Task<IList<Inmueble>> ListarInmuebles(int disponible, int? offset = null, int? limit = null, string? nomApeProp = null);
    public Task<IList<Inmueble>> ListarInmueblesPorPropietario(int idProp, int? offset, int? limit);
    public Task<int> ContarInmuebles(int? disponible);
    public Task<IList<Inmueble>> ListarInmueblesParaAlquilar(string desde, string hasta, string? uso, int? tipo, int? cantAmb, decimal? precio, int offset, int limit);
}