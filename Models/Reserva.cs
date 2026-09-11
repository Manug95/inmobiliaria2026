using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class Reserva
{
    [Display(Name = "N°")]
    public long Id { get; set; }

    [Required(ErrorMessage = "Falta el inquilino")]
    [Range(1, int.MaxValue, ErrorMessage = "ID del inquilino no es correcta")]
    public int? IdInquilino { get; set; }

    public Inquilino? Inquilino { get; set; }

    [Required(ErrorMessage = "Falta el inmueble")]
    [Range(1, int.MaxValue, ErrorMessage = "ID del inmueble no es correcto")]
    public int? IdInmueble { get; set; }

    public Inmueble? Inmueble { get; set; }
    
    [DataType(DataType.Currency)]
    [Display(Name = "Monto por Día")]
    public decimal? Monto { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "Inicio")]
    public DateTime? FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "Fin")]
    [FechaMayorQue(nameof(FechaInicio), ErrorMessage = "La fecha fin no puede ser menor que inicio.")]
    public DateTime? FechaFin { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "F. Terminado")]
    public DateTime? FechaTerminado { get; set; }

    public bool Borrado { get; set; }

    public int IdUsuarioReservador { get; set; }

    public Usuario? UsuarioReservador { get; set; }

    public int IdUsuarioTerminador { get; set; }

    public Usuario? UsuarioTerminador { get; set; }

    public Reserva() { }

    public override string ToString()
    {
        return @$"
        IdInquilino: {IdInquilino}
        IdInmueble: {IdInmueble}
        Monto: {Monto}
        FechaInicio: {FechaInicio}
        FechaFin: {FechaFin}
        FechaTerminado: {FechaTerminado}";
    }
}

public class FechaMayorQueAttribute : ValidationAttribute
{
    private readonly string _otraPropiedad;

    public FechaMayorQueAttribute(string otraPropiedad)
    {
        _otraPropiedad = otraPropiedad;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var propiedad = validationContext.ObjectType.GetProperty(_otraPropiedad);
        if (propiedad == null)
            return new ValidationResult($"La propiedad {_otraPropiedad} no existe.");

        var valorActual = value as DateTime?;
        var valorComparar = propiedad.GetValue(validationContext.ObjectInstance) as DateTime?;

        if (!valorActual.HasValue || !valorComparar.HasValue)
            return ValidationResult.Success;

        if (valorActual.Value <= valorComparar.Value)
            return new ValidationResult(ErrorMessage ?? $"La fecha debe ser mayor que {_otraPropiedad}.");

        return ValidationResult.Success;
    }
}