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
    [Required]
    [Display(Name = "Cant. Ambientes")]
    public int CantidadAmbientes { get; set; }
    [Required]
    [Range(0, double.MaxValue)]
    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }
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
        CantidadAmbientes: {CantidadAmbientes}
        Precio: {Precio}
        Calle: {Calle}
        NroCalle: {NroCalle}
        Latitud: {Latitud}
        Longitud: {Longitud}";
    }
}