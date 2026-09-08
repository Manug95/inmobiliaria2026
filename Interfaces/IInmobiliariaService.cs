using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;

namespace inmobiliaria2026.Interfaces;

public interface IInmobiliariaService
{
    public Task<MultaViewModel> GetMulta(Reserva reserva);
}