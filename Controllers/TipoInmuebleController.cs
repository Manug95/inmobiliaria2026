using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
        IList<TipoInmueble> tiposInmueble = await _repo.ListarAsync(cantidadPaginado, (pagina - 1) * cantidadPaginado);
        int cantidadTiposInmueble = await _repo.ContarTiposInmueble();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadTiposInmueble / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;

        TipoInmuebleViewModel viewModel = new TipoInmuebleViewModel
        {
            TiposInmuebles = tiposInmueble,
            TipoInmueble = new TipoInmueble(),
            MensajeError = TempData["MensajeError"] as string
        };

        return View(viewModel);
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
            string errorMsg = "";
            foreach (var estado in ModelState)
            {
                var campo = estado.Key;
                foreach (var error in estado.Value.Errors)
                    errorMsg += $" - {error.ErrorMessage}";
            }
            TempData["MensajeError"] = errorMsg;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();

        await _repo.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}