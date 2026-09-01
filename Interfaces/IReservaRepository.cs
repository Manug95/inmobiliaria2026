using inmobiliaria2026.Models;

namespace inmobiliaria2026.Interfaces;

public interface IReservaRepository : IRepository<Reserva, long>
{
    public Task<IList<Reserva>> ListarReservas(int? offset, int? limit, int? idInm, string? desde = null, string? hasta = null);
    public Task<int> ContarReservas(int? idInm, string? desde = null, string? hasta = null);
    public Task<bool> EstaOcupado(string desde, string hasta, int inmuebleId, long reservaId);
}