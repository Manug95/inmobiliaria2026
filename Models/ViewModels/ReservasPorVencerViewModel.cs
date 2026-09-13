using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class ReservasPorVencerViewModel
{
    public List<Reserva> Reservas { get; set; } = [];

    [Range(1, 365, ErrorMessage = "Valor mínimo es 1 y el valor máximo es 365")]
    public int? Dias { get; set; }

    [FechaMayorOIgualQueHoy(ErrorMessage = "La fecha Desde no puede ser menor que la fecha actual")]
    public DateTime? Desde { get; set; }

    [FechaMayorQue(nameof(Desde), ErrorMessage = "La fecha Hasta no puede ser menor que la fecha Desde")]
    public DateTime? Hasta { get; set; }
    public bool B { get; set; } = false;
}