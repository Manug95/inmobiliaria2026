using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

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
    [FechaStringMayorQue(nameof(Desde), ErrorMessage = "La fecha Hasta no puede ser menor que Desde.")]
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

public class FechaStringMayorQueAttribute : ValidationAttribute
{
    private readonly string _otraPropiedad;

    public FechaStringMayorQueAttribute(string otraPropiedad)
    {
        _otraPropiedad = otraPropiedad;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        PropertyInfo? propiedad = validationContext.ObjectType.GetProperty(_otraPropiedad);
        if (propiedad == null)
            return new ValidationResult($"La propiedad {_otraPropiedad} no existe en el modelo.");

        var valorActualStr = value as string;
        var valorCompararStr = propiedad.GetValue(validationContext.ObjectInstance) as string;

        if (string.IsNullOrWhiteSpace(valorActualStr) || string.IsNullOrWhiteSpace(valorCompararStr))
            return ValidationResult.Success;

        if (!DateTime.TryParse(valorActualStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var valorActual))
            return new ValidationResult($"La fecha de {validationContext.DisplayName} no es válida.");

        if (!DateTime.TryParse(valorCompararStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var valorComparar))
            return new ValidationResult($"La fecha de {_otraPropiedad} no es válida.");

        if (valorActual < valorComparar)
            return new ValidationResult(ErrorMessage ?? $"La fecha {validationContext.DisplayName} debe ser mayor o igual que {_otraPropiedad}.");

        return ValidationResult.Success;
    }
}