using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class FiltroInmuebleViewModel
{
    public List<Inmueble> Inmuebles { get; set; } = [];
    public List<TipoInmueble> TiposInmuebles { get; set; } = [];
    public Inmueble? Inmueble { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public string? Desde { get; set; }
    [Required(ErrorMessage = "La fecha de fin es requerida")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public string? Hasta { get; set; }
    public int IdInquilino { get; set; }
    [DataType(DataType.Currency)]
    [Range(1, double.MaxValue, ErrorMessage = "El monto máximo debe ser positivo")]
    [Display(Name = "Monto por Día")]
    public decimal? MontoMax { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Tipo de inmueble incorrecto")]
    public int? TipoInmueble { get; set; }
    
    [Display(Name = "Seña")]
    public int? SeniaMaxima { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "El cupo debe ser mayor a 0")]
    public int? Cupo { get; set; }
}