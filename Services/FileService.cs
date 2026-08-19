using inmobiliaria2026.Interfaces;

namespace inmobiliaria2026.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    public async Task<string> GuardarImagen(IFormFile imagen, string nombreImagen)
    {
        string wwwPath = _env.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string fileName = nombreImagen + Path.GetExtension(imagen.FileName);
        string pathCompleto = Path.Combine(path, fileName);

        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
            await imagen.CopyToAsync(stream);

        return Path.Combine("/Uploads", fileName);
    }
}