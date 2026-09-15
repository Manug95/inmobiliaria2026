using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class InmueblesNoOcupadosViewModel
{
    public List<Inmueble> Inmuebles { get; set; } = [];

    [Required(ErrorMessage = "La fecha desde es requerida")]
    [DataType(DataType.Date)]
    public DateTime? Desde { get; set; }

    [Required(ErrorMessage = "La fecha hasta es requerida")]
    [FechaMayorQue(nameof(Desde), ErrorMessage = "La fecha Hasta no puede ser menor que Desde.")]
    [DataType(DataType.Date)]
    public DateTime? Hasta { get; set; }

    public bool B { get; set; } = false;
}