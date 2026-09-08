using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;

namespace inmobiliaria2026.Services;

public class InmobiliariaService(IPagoRepository pagoRepository) : IInmobiliariaService
{
    private readonly IPagoRepository _repoPago = pagoRepository;

    public async Task<MultaViewModel> GetMulta(Reserva reserva)
    {
        DateTime fechaTerminado = reserva.FechaTerminado ?? DateTime.Today;
        int totalDiasReserva = (reserva.FechaFin!.Value - reserva.FechaInicio!.Value).Days;
        int cantidadDiasReservados = (fechaTerminado - reserva.FechaInicio!.Value).Days;
        decimal importeTotalReserva = reserva.Monto!.Value * totalDiasReserva;
        decimal pagado = await _repoPago.SumarImportes(reserva.Id);
        decimal deuda = importeTotalReserva - pagado;

        TimeSpan duracionTotalReserva = reserva!.FechaFin!.Value - reserva!.FechaInicio!.Value;
        DateTime fechaMitad = reserva.FechaInicio.Value + TimeSpan.FromTicks(duracionTotalReserva.Ticks / 2);

        decimal multa = fechaTerminado < fechaMitad 
            ? deuda * (decimal)0.5 
            : deuda * (decimal)0.25;

        return new MultaViewModel
        {
            TotalDiasReserva = totalDiasReserva,
            CantidadDiasReservados = cantidadDiasReservados < 0 ? 0 : cantidadDiasReservados,
            ImporteTotalReserva = importeTotalReserva,
            Pagado = pagado,
            Deuda = deuda,
            Importe = multa,
            ReservaId = reserva.Id
        };
    }
}