using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class TipoInmueble
{   
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El maximo de caracteres es 50")]
    [Required(ErrorMessage="El nombre de tipo es requerido")]
    public string? Tipo { get; set; }

    [StringLength(255, ErrorMessage = "El maximo de caracteres es 255")]
    [Display(Name = "DESCRIPCION")]
    public string? Descripcion { get; set; }

    public TipoInmueble() { }

    public override string ToString() => Tipo ?? "TIPO DE INMUEBLE";
}