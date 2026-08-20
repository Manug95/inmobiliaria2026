using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria2026.Controllers;

public class InquilinoController : Controller
{
    private readonly IInquilinoRepository _repo;

    public InquilinoController(IInquilinoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        IList<Inquilino> inquilinos = await _repo.ListarAsync(cantidadPaginado, pagina);
        int cantidadInquilinos = await _repo.ContarInquilinos();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadInquilinos / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.linkActivo = "inquilinos";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(inquilinos);
    }

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] string? nomApe, [FromQuery] string? orderBy, [FromQuery] string? order, [FromQuery] int? offset = 1, [FromQuery] int? limit = 10)
    {
        IList<Inquilino> inquilinos = await _repo.ListarInquilinos(nomApe, orderBy, order, limit, offset);
        return Json(new { datos = inquilinos });
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromForm] Inquilino inquilino)
    {
        if (ModelState.IsValid)
        {
            if (inquilino.Id > 0)
                await _repo.ActualizarAsync(inquilino);
            else
                await _repo.CrearAsync(inquilino);
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