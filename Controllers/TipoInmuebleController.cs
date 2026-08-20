using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria2026.Controllers;

public class TipoInmuebleController : Controller
{
    private readonly ITipoInmuebleRepository _repo;

    public TipoInmuebleController(ITipoInmuebleRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        IList<TipoInmueble> tiposInmueble = await _repo.ListarAsync(cantidadPaginado, pagina);
        int cantidadTiposInmueble = await _repo.ContarTiposInmueble();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadTiposInmueble / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.linkActivo = "inmuebles";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(tiposInmueble);
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromForm] TipoInmueble tipoInmueble)
    {
        if (ModelState.IsValid)
        {
            if (tipoInmueble.Id > 0)
                await _repo.ActualizarAsync(tipoInmueble);
            else
                await _repo.CrearAsync(tipoInmueble);
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
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