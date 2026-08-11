namespace inmobiliaria2026.Interfaces;

public interface IRepository<T, ID>
{
    public Task<T?> ObtenerPorIdAsync(ID id);
    public Task<List<T>> ListarAsync(int limit, int offset);
    public Task<ID> CrearAsync(T entidad);
    public Task<bool> ActualizarAsync(T entidad);
    public Task<bool> EliminarAsync(ID id);
}