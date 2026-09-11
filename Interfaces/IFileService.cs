namespace inmobiliaria2026.Interfaces;

public interface IFileService
{
    public Task<string> GuardarImagenPortada(IFormFile formFile, string path);
    public Task<string> GuardarImagenInterior(IFormFile imagen, string nombreImagen, int id);
    public Task<string> GuardarAvatarDelUsuario(IFormFile imagen, string nombreImagen);
    public void BorrarImagenInterior(string nombreImagen, int inmuebleId);
    public void BorrarImagenPortada(string nombreImagen);
    public void BorrarAvatar(int id, string avatar);
}