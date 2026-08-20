namespace inmobiliaria2026.Models.ViewModels;

public class InmuebleViewModel
{
    public IList<Inmueble>? Inmuebles { get; set; }
    public Inmueble? Inmueble { get; set; }
    public InmuebleFormData? InmuebleFormData { get; set; }
    public IEnumerable<TipoInmueble>? TiposInmuebles { get; set; }
    public IEnumerable<Propietario>? Propietarios { get; set; }
    public string? MensajeError { get; set; }
    public IEnumerable<string>? Fotos { get; set; } = [];
}