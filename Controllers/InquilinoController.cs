using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

public class InquilinoController : Controller
{
    private readonly IInquilinoRepository _repo;

    public InquilinoController(IInquilinoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(int pagina = 1, int cantidadPaginado = 10)
    {
        IList<Inquilino> inquilinos = await _repo.ListarAsync(cantidadPaginado, (pagina - 1) * cantidadPaginado);
        int cantidadInquilinos = await _repo.ContarInquilinos();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadInquilinos / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;

        InquilinoViewModel viewModel = new InquilinoViewModel
        {
            Inquilinos = inquilinos,
            Inquilino = new Inquilino(),
            MensajeError = TempData["MensajeError"] as string
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Listar(string? nomApe, string? orderBy, string? order, int? offset = 1, int? limit = 10)
    {
        IList<Inquilino> inquilinos = await _repo.ListarInquilinos(nomApe, orderBy, order, limit, (offset - 1) * limit);
        return Json(new { datos = inquilinos });
    }

    public async Task<IActionResult> Guardar(Inquilino inquilino)
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
            string errorMsg = "<ul>";
            foreach (var estado in ModelState)
            {
                var campo = estado.Key;
                foreach (var error in estado.Value.Errors)
                {
                    errorMsg += $"<li class=\"text-danger fs-5\"><strong>{error.ErrorMessage}</strong></li>";
                }
            }
            TempData["MensajeError"] = errorMsg + "</ul>";
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        await _repo.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}