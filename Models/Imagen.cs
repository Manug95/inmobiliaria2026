namespace inmobiliaria2026.Models;

public class Imagen
{
    public long Id { get; set; }
    public string? Ruta { get; set; }
    public List<Imagen> Imagenes { get; set; } = [];
    public int InmuebleId { get; set; }
    public IFormFile? ImagenFile { get; set; }
    public List<IFormFile>? ImagenesFile { get; set; }
}