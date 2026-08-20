using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria2026.Controllers;

public class InmuebleController : Controller
{
    private readonly IInmuebleRepository _repo;
    private readonly ITipoInmuebleRepository _repoTipoInmueble;
    private readonly IPropietarioRepository _repoPropietario;

    public InmuebleController(IInmuebleRepository repo, ITipoInmuebleRepository repoTipoInmueble, IPropietarioRepository repoPropietario)
    {
        _repo = repo;
        _repoPropietario = repoPropietario;
        _repoTipoInmueble = repoTipoInmueble;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? prop, [FromQuery] int idProp = 0, [FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10, [FromQuery] int disp = (int)Disponiblilidad.TODOS)
    {
        if (idProp < 0 || pagina < 0 || cantidadPaginado < 0)
            return BadRequest();

        IList<Inmueble>? inmuebles;
        int cantidadInmuebles = await _repo.ContarInmuebles(disp, idProp);

        if (idProp != 0)
        {
            inmuebles = await _repo.ListarInmueblesPorPropietario(idProp, pagina, cantidadPaginado);
            if (inmuebles.Count != 0)
                ViewBag.propietario = inmuebles.First()?.Duenio?.Apellido + " " + inmuebles.First()?.Duenio?.Nombre;
        }
        else
        {
            inmuebles = await _repo.ListarInmuebles(disp, pagina, cantidadPaginado, prop);
            ViewBag.propietario = prop;
        }
        

        ViewBag.linkActivo = "inmuebles";
        ViewBag.cantPag = Math.Ceiling((decimal)cantidadInmuebles / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.disponible = disp;
        ViewBag.idProp = idProp;

        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(inmuebles);
    }

    /*
        [Bind(Prefix = "InmuebleFormData")] es porque
        el InmuebleViewModel que le paso a la vista tiene como atributo un InmuebleFormData
        y al generar el HTML, los atributos name de los inputs se crean con este formato "InmuebleFormData.nombredelcampo"
        entoces con este Bind le digo al framework que tenga en cuenta eso para poder mapear los campos del formulario correctamente
    */
    [HttpPost]
    public async Task<IActionResult> Guardar([Bind(Prefix = nameof(InmuebleFormData))] [FromForm] InmuebleFormData inmuebleForm, [FromServices] IFileService fileService)
    {
        if (ModelState.IsValid)
        {
            if (inmuebleForm.IdTipoInmueble == 0)
            {
                int idTipoInmuebleNuevo = await _repoTipoInmueble.CrearAsync(
                    new TipoInmueble
                    {
                        Tipo = inmuebleForm.NuevoTipo,
                        Descripcion = inmuebleForm.NuevoTipoDescripcion
                    }
                );
                inmuebleForm.IdTipoInmueble = idTipoInmuebleNuevo;
            }

            Inmueble inmueble = inmuebleForm.GetInmueble();

            await _repo.CrearAsync(inmueble);

            if (inmuebleForm.FotoFile != null)
            {
                if (!inmuebleForm.FotoFile.ContentType.Contains("image/"))
                    return BadRequest();
                
                string portadaURL = await fileService.GuardarImagen(inmuebleForm.FotoFile, "foto_" + inmueble.Id);
                inmueble.Foto = portadaURL;
                await _repo.ActualizarAsync(inmueble);
            }
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Actualizar([Bind(Prefix = nameof(InmuebleFormData))] [FromForm] InmuebleFormData inmuebleForm, [FromServices] IFileService fileService)
    {
        if (ModelState.IsValid)
        {
            if (inmuebleForm.IdTipoInmueble == 0)
            {
                int idTipoInmuebleNuevo = await _repoTipoInmueble.CrearAsync(
                    new TipoInmueble
                    {
                        Tipo = inmuebleForm.NuevoTipo,
                        Descripcion = inmuebleForm.NuevoTipoDescripcion
                    }
                );
                inmuebleForm.IdTipoInmueble = idTipoInmuebleNuevo;
            }

            Inmueble? inmueble = await _repo.ObtenerPorIdAsync(inmuebleForm.Id);
            if (inmueble == null)
                return BadRequest();

            inmueble.Calle = inmuebleForm.Calle;
            inmueble.Cupo = (int)inmuebleForm.Cupo!;
            inmueble.IdPropietario = inmuebleForm.IdPropietario;
            inmueble.IdTipoInmueble = inmuebleForm.IdTipoInmueble;
            inmueble.Latitud = inmuebleForm.Latitud != null ? (decimal)inmuebleForm.Latitud : 0;
            inmueble.Longitud = inmuebleForm.Longitud != null ? (decimal)inmuebleForm.Longitud : 0;
            inmueble.NroCalle = (uint)inmuebleForm.NroCalle!;
            inmueble.Precio = (decimal)inmuebleForm.Precio!;
            inmueble.Senia = inmuebleForm.Senia;
            inmueble.Disponible = inmuebleForm.Disponible;

            if (inmuebleForm.FotoFile != null)
            {
                if (!inmuebleForm.FotoFile.ContentType.Contains("image/"))
                    return BadRequest();
                
                string portadaURL = await fileService.GuardarImagen(inmuebleForm.FotoFile, "portada_" + inmueble.Id);
                inmueble.Foto = portadaURL;
            }
            await _repo.ActualizarAsync(inmueble);
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario([FromRoute] int id = 0, [FromQuery] int idProp = 0)
    {
        IList<TipoInmueble> tiposInmuebles = await _repoTipoInmueble.ListarAsync(10, 0);
        IList<Propietario> propietarios = [];

        if (idProp > 0)
        {
            Propietario? prop = await _repoPropietario.ObtenerPorIdAsync(idProp);
            propietarios.Add(prop!);
        }

        InmuebleFormData? inmuebleFormData = null;

        if (id > 0)
        {
            Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);
            if (inmueble != null)
            {
                inmuebleFormData = new InmuebleFormData
                {
                    Id = inmueble.Id,
                    Calle = inmueble.Calle,
                    Cupo = inmueble.Cupo,
                    IdPropietario = inmueble.IdPropietario,
                    IdTipoInmueble = inmueble.IdTipoInmueble,
                    Latitud = inmueble.Latitud,
                    Longitud = inmueble.Longitud,
                    NroCalle = inmueble.NroCalle,
                    Precio = inmueble.Precio,
                    Senia = inmueble.Senia,
                    Disponible = inmueble.Disponible,
                    Duenio = inmueble.Duenio,
                    Foto = inmueble.Foto
                };
            }

        }

        ViewBag.linkActivo = "inmuebles";
        ViewBag.accion = id > 0 ? "Actualizar" : "Guardar";

        InmuebleViewModel viewModel = new InmuebleViewModel
        {
            Inmueble = new Inmueble(),
            TiposInmuebles = tiposInmuebles,
            Propietarios = propietarios,
            InmuebleFormData = inmuebleFormData ?? new InmuebleFormData()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();

        if (!await _repo.EliminarAsync(id))
            TempData["MensajeError"] = "No se pudo borrar el registro";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Buscar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();
        
        Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);
        return Json(new { inmueble });
    }

    [HttpGet]
    public async Task<IActionResult> Detalle([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();
        
        Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);
        IList<string> fotos = [];
        
        return View(new DetalleInmuebleViewModel(inmueble, fotos));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private string ModelStateError(ModelStateDictionary modelState)
    {
        string errorMsg = "";
        foreach (var estado in modelState)
        {
            var campo = estado.Key;
            foreach (var error in estado.Value.Errors)
            {
                errorMsg += $"{error.ErrorMessage}</br>";
            }
        }
        return errorMsg;
    }
}