namespace inmobiliaria2026.Models.ViewModels;

public class TipoInmuebleViewModel
{
    public IEnumerable<TipoInmueble>? TiposInmuebles { get; set; }
    public TipoInmueble? TipoInmueble { get; set; }
    public string? MensajeError { get; set; }
}