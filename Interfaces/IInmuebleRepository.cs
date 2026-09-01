using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IInmuebleRepository : IRepository<Inmueble, int>
{
    public Task<IList<Inmueble>> ListarInmuebles(int disponible, int? offset, int? limit, string? nomApeProp);
    public Task<IList<Inmueble>> ListarInmueblesPorPropietario(int idProp, int? offset, int? limit);
    public Task<int> ContarInmuebles(int? disponible, int? idProp);
    public Task<long> ContarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio);
    public Task<List<Inmueble>> ListarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio, int offset, int limit);
    public Task<bool> GuardarImagen(int inmuebleId, string ruta);
    public Task<bool> EliminarImagen(string ruta);
}