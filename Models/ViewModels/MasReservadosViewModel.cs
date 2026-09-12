using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class MasReservadosViewModel
{
    [Required(ErrorMessage = "Debe ingresar los días")]
    [Range(30, 3650, ErrorMessage = "Mínimo 30 - Máximo 3650")]
    public int Dias { get; set; } = 365;

    [Range(3, 10, ErrorMessage = "La cantidad debe estar entre 3 y 10")]
    public int Cantidad { get; set; } = 10;

    public List<Inmueble> Inmuebles { get; set; } = [];
}