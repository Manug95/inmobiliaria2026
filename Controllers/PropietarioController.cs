using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

public class PropietarioController : Controller
{
    private readonly IPropietarioRepository _repo;

    public PropietarioController(IPropietarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index(int pagina = 1, int cantidadPaginado = 10)
    {
        IList<Propietario> propietarios = await _repo.ListarAsync(cantidadPaginado, (pagina - 1) * cantidadPaginado);
        int cantidadPropietarios = await _repo.ContarPropietarios();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadPropietarios / cantidadPaginado);
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;

        PropietarioViewModel viewModel = new PropietarioViewModel
        {
            Propietarios = propietarios,
            Propietario = new Propietario(),
            MensajeError = TempData["MensajeError"] as string
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Listar(string? nomApe, string? orderBy, string? order, int? offset = 1, int? limit = 10)
    {
        IList<Propietario> propietarios = await _repo.ListarPropietarios(nomApe, orderBy, order, (offset - 1) * limit, limit);
        return Json(new { datos = propietarios });
    }

    public IActionResult Guardar(Propietario propietario)
    {
        if (ModelState.IsValid)
        {
            if (propietario.Id > 0)
            {
                _repo.ActualizarAsync(propietario);
            }
            else
            {
                _repo.CrearAsync(propietario);
            }
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
            TempData["MensajeError"] = errorMsg + "";
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Eliminar(int id)
    {
        _repo.EliminarAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}