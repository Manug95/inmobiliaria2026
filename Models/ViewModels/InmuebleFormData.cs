using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class InmuebleFormData
{
    [Display(Name = "Nro Inmueble")]
    [Range(0, int.MaxValue, ErrorMessage = "ID del inmueble incorrecta")]
    public int Id { get; set; }

    [Required]
    public int IdPropietario { get; set; }

    [Display(Name = "Dueño")]
    public Propietario? Duenio { get; set; }

    [Required]
    public int IdTipoInmueble { get; set; }

    [Display(Name = "Tipo Inmueble")]
    public TipoInmueble? Tipo { get; set; }

    [Required]
    public int? Cupo { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal? Precio { get; set; }
    [Required]
    [Range(1, 100, ErrorMessage = "El porcentaje debe estan entre 1 y 100")]
    public int Senia { get; set; }

    [Required]
    public string? Calle { get; set; }

    [Required]
    public uint? NroCalle { get; set; }

    public decimal? Latitud { get; set; }

    public decimal? Longitud { get; set; }

    public bool Disponible { get; set; }

    public string? NuevoTipo { get; set; }
    public string? NuevoTipoDescripcion { get; set; }
    public IFormFile? FotoFile { get; set; }
    public string? Foto { get; set; }

    public InmuebleFormData() { }

    public Inmueble GetInmueble()
    {
        return new Inmueble
        {
            Calle = Calle,
            Cupo = (int)Cupo!,
            IdPropietario = IdPropietario,
            IdTipoInmueble = IdTipoInmueble,
            Latitud = Latitud != null ? (decimal)Latitud : 0,
            Longitud = Longitud != null ? (decimal)Longitud : 0,
            NroCalle = (uint)NroCalle!,
            Precio = (decimal)Precio!,
            Senia = Senia!,
            Foto = Foto,
            Disponible = Disponible
        };
    }

    public override string ToString()
    {
        return $"Dirección {Calle} {NroCalle}{(Duenio != null ? $" perteneciente a {Duenio?.ToString()}" : "")}";
    }
}