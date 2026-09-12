using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

[Authorize]
public class InmuebleController : ControladorBase
{
    private readonly IInmuebleRepository _repo;
    private readonly ITipoInmuebleRepository _repoTipoInmueble;
    private readonly IPropietarioRepository _repoPropietario;
    private readonly IImagenRepository _repoImagenes;

    public InmuebleController(
        IInmuebleRepository repo, 
        ITipoInmuebleRepository repoTipoInmueble, 
        IPropietarioRepository repoPropietario,
        IImagenRepository repoImagenes
    )
    {
        _repo = repo;
        _repoPropietario = repoPropietario;
        _repoTipoInmueble = repoTipoInmueble;
        _repoImagenes = repoImagenes;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] string? prop, [FromQuery] int idProp = 0, [FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10, [FromQuery] int disp = (int)Disponiblilidad.TODOS)
    {
        if (idProp < 0 || pagina <= 0 || cantidadPaginado <= 0 || !Enum.IsDefined(typeof(Disponiblilidad), disp))
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
        ViewBag.cantidadPaginado = cantidadPaginado;
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
    public async Task<IActionResult> Guardar([Bind(Prefix = nameof(InmuebleFormData))] [FromForm] InmuebleFormData inmuebleForm)
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

            await _repo.CrearAsync(inmuebleForm.GetInmueble());
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Actualizar([Bind(Prefix = nameof(InmuebleFormData))] [FromForm] InmuebleFormData inmuebleForm)
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

            await _repo.ActualizarAsync(inmueble);
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> GuardarImagenes([FromForm] Imagen imagenForm, [FromServices] IFileService fileService)
    {
        if (imagenForm.ImagenFile != null && imagenForm.ImagenFile.ContentType.Contains("image/"))
        {
            Inmueble? inmueble = await _repo.ObtenerPorIdAsync(imagenForm.InmuebleId);
            if (inmueble != null)
            {
                try
                {
                    if (inmueble.Foto != null)
                    {
                        fileService.BorrarImagenPortada(inmueble.Foto); //Path.GetFileName(inmueble.Foto)
                    }
                    string portadaURL = await fileService.GuardarImagenPortada(imagenForm.ImagenFile, "portada_" + imagenForm.InmuebleId);
                    inmueble.Foto = portadaURL;
                    await _repo.ActualizarAsync(inmueble);
                }
                catch (Exception e)
                {
                    TempData["MensajeError"] = e.Message;
                    return RedirectToAction(nameof(FormulariosImagenes), new { id = imagenForm.InmuebleId });
                }
            }
        }

        if (imagenForm.ImagenesFile != null && imagenForm.ImagenesFile.Count != 0)
        {
            string nombreArchivo = "";
            try
            {
                string ImagenURL;
                foreach(var foto in imagenForm.ImagenesFile)
                {
                    nombreArchivo = foto.FileName;
                    if (foto.ContentType.Contains("image/"))
                    {
                        ImagenURL = await fileService.GuardarImagenInterior(foto, Guid.NewGuid().ToString(), imagenForm.InmuebleId);
                        await _repoImagenes.CrearAsync(new Imagen()
                        {
                            InmuebleId = imagenForm.InmuebleId,
                            Ruta = ImagenURL
                        });
                    }
                }
            }
            catch (Exception e)
            {
                TempData["MensajeError"] = $"Error en la imagen '{nombreArchivo}', {e.Message}";
                return RedirectToAction(nameof(FormulariosImagenes), new { id = imagenForm.InmuebleId });
            }
        }

        return RedirectToAction(nameof(FormulariosImagenes), new { id = imagenForm.InmuebleId });
    }

    [HttpGet]
    public async Task<IActionResult> Formulario([FromRoute] int id = 0, [FromQuery] int idProp = 0)
    {
        IList<TipoInmueble> tiposInmuebles = await _repoTipoInmueble.ListarAsync(10, 1);
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

    [HttpGet]
    public async Task<IActionResult> FormulariosImagenes([FromRoute] int id = 0)
    {
        Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);
        List<Imagen> imagenes = await _repoImagenes.ListarPorInmuebleAsync(id, 10, 1);

        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(new Imagen()
        {
            Ruta = inmueble?.Foto,
            InmuebleId = inmueble!.Id,
            Imagenes = imagenes
        });
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();

        if (!await _repo.EliminarAsync(id))
            TempData["MensajeError"] = "No se pudo borrar el registro";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> EliminarPortada([FromRoute] int id, [FromServices] IFileService fileService)
    {
        if (id <= 0)
            return BadRequest();

        try
        {
            Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);

            if (inmueble == null)
                return NotFound();

            if (inmueble.Foto != null)
            {
                fileService.BorrarImagenPortada(inmueble.Foto);

                inmueble.Foto = null;
                await _repo.ActualizarAsync(inmueble);
            }
        }
        catch (Exception e)
        {
            TempData["MensajeError"] = $"{e.Message}";
        }

        return RedirectToAction(nameof(FormulariosImagenes), new { id });
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> EliminarImagenInterior([FromRoute] int id, [FromForm] Imagen imagen, [FromServices] IFileService fileService)
    {
        if (id <= 0)
            return BadRequest();

        if (await _repoImagenes.EliminarAsync(id))
        {
            try
            {
                fileService.BorrarImagenInterior(Path.GetFileName(imagen.Ruta!), imagen.InmuebleId);
            }
            catch (Exception)
            {
                TempData["MensajeError"] = "No se pudo borrar la imagen";
            }
        } 
        else
            TempData["MensajeError"] = "No se pudo borrar la imagen";

        

        return RedirectToAction(nameof(FormulariosImagenes), new { id = imagen.InmuebleId });
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
        List<Imagen> imagenes = await _repoImagenes.ListarPorInmuebleAsync(id, 100, 1);
        List<string> fotos = [.. imagenes.Select(i => i.Ruta!)];
        
        // return View(new DetalleInmuebleViewModel(inmueble, fotos));
        return View(new DetalleInmuebleViewModel(inmueble, imagenes));
    }

    [HttpGet]
    public async Task<IActionResult> Reservar(
        [FromQuery] FiltroInmuebleViewModel filtros, 
        [FromQuery] int pagina = 1, 
        [FromQuery] int cantidadPaginado = 10
    )
    {
        List<Inmueble> inmuebles = [];
        List<TipoInmueble>? tipoInmuebles = await _repoTipoInmueble.ListarAsync(100, 1);

        if (!ModelState.IsValid)
        {
            filtros.TiposInmuebles = tipoInmuebles;
            filtros.Inmuebles = inmuebles;
            return View(filtros);
        }

        if (filtros.Desde != null && filtros.Hasta != null)
        {
            inmuebles = await _repo.ListarInmueblesParaAlquilar(filtros.Desde, filtros.Hasta, filtros.TipoInmueble, filtros.Cupo, filtros.MontoMax, pagina, cantidadPaginado);
            long cantidadInmuebles = await _repo.ContarInmueblesParaAlquilar(filtros.Desde, filtros.Hasta, filtros.TipoInmueble, filtros.Cupo, filtros.MontoMax);

            if (inmuebles.Count == 0)
                ViewBag.Mensaje = "No se encontraron resultados";
            
            ViewBag.cantPag = (int)Math.Ceiling((decimal)cantidadInmuebles / cantidadPaginado);
            ViewBag.cantidadPaginado = cantidadPaginado;
            ViewBag.paginaSiguiente = pagina + 1;
            ViewBag.paginaAnterior = pagina - 1;
            ViewBag.linkActivo = "inmuebles";
        }

        filtros.TiposInmuebles = tipoInmuebles;
        filtros.Inmuebles = inmuebles;
        return View(filtros);
    }

    [HttpGet]
    public async Task<IActionResult> MasReservados([FromQuery] MasReservadosViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var inmuebles = await _repo.ListarMasReservadosUltimosXDias(vm.Dias, vm.Cantidad);
        if (inmuebles.Count > 0)
        {
            vm.Inmuebles = inmuebles;
        }
        else
        {
            ViewBag.MensajeError = "No se encontraro resultados";
        }

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}