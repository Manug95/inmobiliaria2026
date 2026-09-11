using inmobiliaria2026.Interfaces;

namespace inmobiliaria2026.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _env;
    private readonly string PATH_UPLOADS;
    private readonly string PATH_ABSOLUTO_INMUEBLES;
    private readonly string PATH_RELATIVO_INMUEBLES;
    private readonly string PATH_ABSOLUTO_AVATARES;
    private readonly string PATH_RELATIVO_AVATARES;

    public FileService(IWebHostEnvironment env)
    {
        _env = env;
        PATH_UPLOADS = Path.Combine(_env.WebRootPath, "Uploads");;
        PATH_ABSOLUTO_INMUEBLES = Path.Combine(PATH_UPLOADS, "Inmuebles");
        PATH_RELATIVO_INMUEBLES = Path.Combine("/Uploads", "Inmuebles");
        PATH_ABSOLUTO_AVATARES = Path.Combine(PATH_UPLOADS, "Avatares");
        PATH_RELATIVO_AVATARES = Path.Combine("/Uploads", "Avatares");
    }
    
    public async Task<string> GuardarImagenPortada(IFormFile imagen, string nombreImagen)
    {
        try
        {
            CrearDirectoriosSiNoExisten();

            string fileName = nombreImagen + Path.GetExtension(imagen.FileName);
            string pathCompleto = Path.Combine(PATH_ABSOLUTO_INMUEBLES, fileName);

            EscribirArchivo(imagen, pathCompleto);

            return Path.Combine(PATH_RELATIVO_INMUEBLES, fileName);
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Falta el nombre de la imagen");
        }
        catch (ArgumentException)
        {
            throw new Exception("");
        }
    }

    public async Task<string> GuardarImagenInterior(IFormFile imagen, string nombreImagen, int inmuebleId)
    {
        try
        {
            CrearDirectoriosSiNoExisten();

            string pathInmuebleId = Path.Combine(PATH_ABSOLUTO_INMUEBLES, inmuebleId.ToString());

            if (!Directory.Exists(pathInmuebleId))
                Directory.CreateDirectory(pathInmuebleId);

            string fileName = nombreImagen + Path.GetExtension(imagen.FileName);
            string pathCompleto = Path.Combine(pathInmuebleId, fileName);

            EscribirArchivo(imagen, pathCompleto);

            return Path.Combine(PATH_RELATIVO_INMUEBLES, inmuebleId.ToString(), fileName);
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Falta el nombre de la imagen o la id del inmueble");
        }
        catch (ArgumentException)
        {
            throw new Exception("");
        }
    }

    public void BorrarImagenInterior(string nombreImagen, int inmuebleId)
    {
        try
        {
            BorrarArchivo(Path.Combine(PATH_ABSOLUTO_INMUEBLES, inmuebleId.ToString(), nombreImagen));
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Falta el nombre de la imagen o la id del inmueble");
        }
        catch (ArgumentException)
        {
            throw new Exception("");
        }
    }

    public void BorrarImagenPortada(string ruta)
    {
        try
        {
            string pathPortada = Path.Combine(PATH_ABSOLUTO_INMUEBLES, Path.GetFileName(ruta));
            if (Directory.Exists(PATH_ABSOLUTO_INMUEBLES) && File.Exists(pathPortada))
                BorrarArchivo(pathPortada);
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Falta la ruta de la imagen");
        }
        catch (ArgumentException)
        {
            throw new Exception("");
        }
    }

    public async Task<string> GuardarAvatarDelUsuario(IFormFile imagen, string nombreImagen)
    {
        string wwwPath = _env.WebRootPath;
        string path = Path.Combine(wwwPath, "Uploads");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Combine(path, "Avatares");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string fileName = nombreImagen + Path.GetExtension(imagen.FileName);
        string pathCompleto = Path.Combine(path, fileName);

        using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
        {
            await imagen.CopyToAsync(stream);
        }

        return Path.Combine("/Uploads", "Avatares", fileName);
    }

    public void BorrarAvatar(int id, string avatar)
    {
        try
        {
            var ruta = Path.Combine(PATH_ABSOLUTO_AVATARES, $"avatar_{id}" + Path.GetExtension(avatar));
            if (File.Exists(ruta))
                File.Delete(ruta);
        }
        catch (ArgumentNullException)
        {
            throw new Exception("Falta la ruta de la imagen");
        }
        catch (ArgumentException)
        {
            throw new Exception("");
        }
    }

    private static async void EscribirArchivo(IFormFile file, string path)
    {
        using (FileStream stream = new FileStream(path, FileMode.Create))
            await file.CopyToAsync(stream);
    }

    private static void BorrarArchivo(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (PathTooLongException)
        {
            throw new Exception("La ruta del archivo es muy larga");
        }
        catch (IOException)
        {
            throw new Exception("No se pudo escribir el archivo");
        }
        catch (NotSupportedException)
        {
            throw new Exception("Operación no soportada");
        }
        catch (UnauthorizedAccessException)
        {
            throw new Exception("El sistema no tiene permisos para borrar el archivo");
        }
    }

    private void CrearDirectoriosSiNoExisten()
    {
        if (!Directory.Exists(PATH_UPLOADS))
            Directory.CreateDirectory(PATH_UPLOADS);

        if (!Directory.Exists(PATH_ABSOLUTO_INMUEBLES))
            Directory.CreateDirectory(PATH_ABSOLUTO_INMUEBLES);
    }
}