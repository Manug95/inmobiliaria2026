namespace inmobiliaria2026.Models.ViewModels;

public class DetalleInmuebleViewModel
{
    public Inmueble? Inmueble { get; set; }
    public IList<string> Fotos { get; set; } = [];
    public List<Imagen> Imagenes { get; set; } = [];

    public DetalleInmuebleViewModel(Inmueble? inmueble, IList<string> fotos)
    {
        Inmueble = inmueble;
        Fotos = fotos;
    }

    public DetalleInmuebleViewModel(Inmueble? inmueble, List<Imagen> imagenes)
    {
        Inmueble = inmueble;
        Imagenes = imagenes;
    }
}