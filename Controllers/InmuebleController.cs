using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

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

        return View(new InmuebleViewModel
            {
                Inmuebles = inmuebles,
                MensajeError = TempData["MensajeError"] as string
            }
        );
    }

    /*
        [Bind(Prefix = "InmuebleFormData")] es porque
        el InmuebleViewModel que le paso a la vista tiene como atributo un InmuebleFormData
        y al generar el HTML, los atributos name de los inputs se crean con este formato "InmuebleFormData.nombredelcampo"
        entoces con este Bind le digo al framework que tenga en cuenta eso para poder mapear los campos del formulario correctamente
    */
    [HttpPost]
    public async Task<IActionResult> Guardar([Bind(Prefix = "InmuebleFormData")] [FromForm] InmuebleFormData inmuebleForm)
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

        if (ModelState.IsValid)
        {
            Inmueble inmueble = new Inmueble
            {
                Calle = inmuebleForm.Calle,
                Cupo = (int)inmuebleForm.Cupo!,
                IdPropietario = inmuebleForm.IdPropietario,
                IdTipoInmueble = inmuebleForm.IdTipoInmueble,
                Latitud = inmuebleForm.Latitud != null ? (decimal)inmuebleForm.Latitud : 0,
                Longitud = inmuebleForm.Longitud != null ? (decimal)inmuebleForm.Longitud : 0,
                NroCalle = (uint)inmuebleForm.NroCalle!,
                Precio = (decimal)inmuebleForm.Precio!,
                Senia = inmuebleForm.Senia!,
                // Foto = inmuebleForm.Foto,
            };

            if (inmuebleForm.Id > 0)
            {
                inmueble.Id = inmuebleForm.Id;
                inmueble.Disponible = inmuebleForm.Disponible;
                await _repo.ActualizarAsync(inmueble);
            }
            else
                await _repo.CrearAsync(inmueble);
        }
        else
        {
            string errorMsg = "";
            foreach (var estado in ModelState)
            {
                var campo = estado.Key;
                foreach (var error in estado.Value.Errors)
                {
                    errorMsg += $" - {error.ErrorMessage}";
                }
            }
            TempData["MensajeError"] = errorMsg;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> FormularioInmueble(int id = 0, int idProp = 0)
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
                    Duenio = inmueble.Duenio
                };
            }

        }

        ViewBag.linkActivo = "inmuebles";

        InmuebleViewModel viewModel = new InmuebleViewModel
        {
            Inmueble = new Inmueble(),
            TiposInmuebles = tiposInmuebles,
            Propietarios = propietarios,
            InmuebleFormData = inmuebleFormData ?? new InmuebleFormData()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        if (id <= 0)
            return BadRequest();

        await _repo.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Buscar(int id)
    {
        if (id <= 0)
            return BadRequest();
        
        Inmueble? inmueble = await _repo.ObtenerPorIdAsync(id);
        return Json(new { inmueble });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}