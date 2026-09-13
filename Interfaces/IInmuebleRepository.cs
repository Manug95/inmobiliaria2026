using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IInmuebleRepository : IRepository<Inmueble, int>
{
    public Task<IList<Inmueble>> ListarInmuebles(int disponible, int? offset, int? limit, string? nomApeProp);
    public Task<IList<Inmueble>> ListarInmueblesPorPropietario(int idProp, int? offset, int? limit);
    public Task<int> ContarInmuebles(int? disponible, int? idProp);
    public Task<int> ContarNoReservados(int dias);
    public Task<long> ContarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio);
    public Task<List<Inmueble>> ListarInmueblesParaAlquilar(string desde, string hasta, int? tipo, int? cupo, decimal? precio, int offset, int limit);
    public Task<bool> GuardarImagen(int inmuebleId, string ruta);
    public Task<bool> EliminarImagen(string ruta);
    public Task<List<Inmueble>> ListarMasReservadosUltimosXDias(int dias, int limit);
    public Task<List<Inmueble>> ListarNoReservados(int dias = 30, int offset = 1, int limit = 10);
}