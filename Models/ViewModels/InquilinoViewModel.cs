namespace inmobiliaria2026.Models.ViewModels;

public class InquilinoViewModel
{
    public IEnumerable<Inquilino>? Inquilinos { get; set; }
    public Inquilino? Inquilino { get; set; }
    public string? MensajeError { get; set; }
    public string? Error { get; set; }
}