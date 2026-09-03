namespace inmobiliaria2026.Interfaces;

public interface IFileService
{
    public Task<string> GuardarImagenPortada(IFormFile formFile, string path);
    public Task<string> GuardarImagenInterior(IFormFile imagen, string nombreImagen, int id);
    public void BorrarImagenInterior(string nombreImagen, int inmuebleId);
    public void BorrarImagenPortada(string nombreImagen);
}