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
    public DateTime? FechaFin { get; set; }

    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    [Display(Name = "F. Terminado")]
    public DateTime? FechaTerminado { get; set; }

    public bool Borrado { get; set; }

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