using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class ReservasPorFechasViewModel
{
    public List<Reserva> Reservas { get; set; } = [];
    public Reserva Reserva { get; set; } = new();

    [Required(ErrorMessage = "La fecha desde es requerida")]
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }
    [Required(ErrorMessage = "La fecha hasta es requerida")]
    [FechaMayorQue(nameof(Desde), ErrorMessage = "La fecha Hasta no puede ser menor que Desde.")]
    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }
}