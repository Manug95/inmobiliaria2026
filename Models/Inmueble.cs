using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class Inmueble
{
    [Display(Name = "Nro Inmueble")]
    public int Id { get; set; }
    [Required]
    public int IdPropietario { get; set; }
    [Display(Name = "Dueño")]
    public Propietario? Duenio { get; set; }
    [Required]
    public int IdTipoInmueble { get; set; }
    [Display(Name = "Tipo Inmueble")]
    public TipoInmueble? Tipo { get; set; }
    [Required(ErrorMessage = "El cupo es requerido")]
    [Display(Name = "Cupo")]
    public int Cupo { get; set; }
    [Required]
    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }
    [Required]
    [Range(1, 100, ErrorMessage = "El porcentaje debe estan entre 1 y 100")]
    [Display(Name = "Seña")]
    public int Senia { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "El maximo de caracteres es 100")]
    public string? Calle { get; set; }
    [Required]
    public uint NroCalle { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public bool Disponible { get; set; }
    public bool Borrado { get; set; }
    [DataType(DataType.ImageUrl)]
    [Display(Name = "Portada")]
    public string? Foto { get; set; }

    public Inmueble() { }

    public override string ToString()
    {
        return $"Dirección {Calle} {NroCalle}{(Duenio != null ? $" perteneciente a {Duenio?.ToString()}" : "")}";
    }

    public string Direccion()
    {
        return $"{Calle} {NroCalle}";
    }

    public string MostrarDatos()
    {
        return @$"
        Id: {Id}
        IdPropietario: {IdPropietario}
        IdTipoInmueble: {IdTipoInmueble}
        Cupo: {Cupo}
        Precio: {Precio}
        Calle: {Calle}
        NroCalle: {NroCalle}
        Latitud: {Latitud}
        Longitud: {Longitud}";
    }
}