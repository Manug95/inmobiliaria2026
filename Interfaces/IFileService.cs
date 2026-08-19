namespace inmobiliaria2026.Interfaces;

public interface IFileService
{
    public Task<string> GuardarImagen(IFormFile formFile, string path);
}