using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class Pago
{
    [Display(Name = "Nro Pago")]
    public long Id { get; set; }

    [Required(ErrorMessage = "La ID de la reserva es requerida")]
    public long ReservaId { get; set; }

    public Reserva? Reserva { get; set; }

    [Required(ErrorMessage = "La fecha del pago es requerida")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
    public DateTime? Fecha { get; set; }

    [Required(ErrorMessage = "El importe es requerido")]
    [Range(1, double.MaxValue, ErrorMessage = "El importe debe ser mayor a 0")]
    [DataType(DataType.Currency)]
    public decimal? Importe { get; set; }

    [MaxLength(255, ErrorMessage = "La cantidad máxima de caracteres es de 255")]
    [DataType(DataType.MultilineText)]
    public string? Concepto { get; set; }

    public bool Anulado { get; set; }
}