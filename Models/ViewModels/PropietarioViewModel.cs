namespace inmobiliaria2026.Models.ViewModels;
public class PropietarioViewModel
{
    public IEnumerable<Propietario>? Propietarios { get; set; }
    public Propietario? Propietario { get; set; }
    public string? MensajeError { get; set; }
    public string? Error { get; set; }
}