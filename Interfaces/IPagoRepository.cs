using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IPagoRepository : IRepository<Pago, long>
{
    public Task<long> ContarPagos(long reservaId);
    public Task<long> ContarPagosDeAlquileres(long reservaId);
    public Task<decimal> SumarImportes(long reservaId);
    public Task<List<Pago>> ListarPagos(int? offset, int? limit, long reservaId);
    public Task<bool> EliminarAsync(long id, int usuarioId);
}