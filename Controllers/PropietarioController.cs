using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria2026.Controllers;

public class PropietarioController : Controller
{
    private readonly IPropietarioRepository _repo;

    public PropietarioController(IPropietarioRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        IList<Propietario> propietarios = await _repo.ListarAsync(cantidadPaginado, pagina);
        int cantidadPropietarios = await _repo.ContarPropietarios();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadPropietarios / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.linkActivo = "propietarios";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(propietarios);
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? nomApe, [FromQuery] string? orderBy, [FromQuery] string? order, [FromQuery] int? offset = 1, [FromQuery] int? limit = 10)
    {
        IList<Propietario> propietarios = await _repo.ListarPropietarios(nomApe, orderBy, order, offset, limit);
        return Json(new { datos = propietarios });
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromForm] Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            if (propietario.Id > 0)
                await _repo.ActualizarAsync(propietario);
            else
                await _repo.CrearAsync(propietario);
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