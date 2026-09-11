using System.Diagnostics;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

[Authorize]
public class ReservaController : ControladorBase
{
    private readonly IReservaRepository _repo;
    private readonly IInmuebleRepository _repoInmueble;
    private readonly IInquilinoRepository _repoInquilino;

    public ReservaController(IReservaRepository repo, IInmuebleRepository repoInmueble, IInquilinoRepository repoInquilino)
    {
        _repo = repo;
        _repoInmueble = repoInmueble;
        _repoInquilino = repoInquilino;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int? idInm, [FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        if ((idInm.HasValue && idInm.Value < 0) || pagina <= 0 || cantidadPaginado <= 0)
            return BadRequest();
            
        IList<Reserva> reservas = await _repo.ListarReservas(pagina, cantidadPaginado, idInm);
        int cantidadReservas = await _repo.ContarReservas(idInm);

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadReservas / cantidadPaginado);
        ViewBag.cantidadPaginado = cantidadPaginado;
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.idInm = idInm;
        ViewBag.contratos = reservas;
        ViewBag.linkActivo = "reservas";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(reservas);
    }

    [HttpGet]
    public async Task<IActionResult> Buscar([FromRoute] int id)
    {
        Reserva? reserva = await _repo.ObtenerPorIdAsync(id);
        return Json(reserva);
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromForm] Reserva reserva, [FromServices] IInmobiliariaService inmobiliariaService)
    {
        var userId = inmobiliariaService.GetUserId(User);
        if (!userId.HasValue)
            return Unauthorized();
        
        if (ModelState.IsValid)
        {
            DateTime desde = reserva.FechaInicio!.Value;
            DateTime hasta = reserva.FechaFin!.Value;
            if (await _repo.EstaOcupado(desde.ToString("yyyy-MM-dd"), hasta.ToString("yyyy-MM-dd"), (int)reserva.IdInmueble!, reserva.Id))
            {
                TempData["MensajeError"] = $"El Inmueble ya está reservado entre {desde:dd-MM-yyyy} y {hasta:dd-MM-yyyy}";
                return RedirectToAction(
                    nameof(Formulario), 
                    new { desde = desde.ToString("yyyy-MM-dd"), hasta = hasta.ToString("yyyy-MM-dd"), idInq = reserva.IdInquilino, idInm = reserva.IdInmueble }
                );
            }

            if (reserva.Id > 0)
            {
                /*
                    al actualizar hay confilcto con las fechas nuevas y las fechas de la misma reserva.
                    es decir, cuand reviso si estan ocupadas las nuevas fechas, me puede salir que el mismo inmueble es el que ocupa alguna las nuevas fechas.
                    se me ocurre joinear la reserva con el inquilino y ver que las ids sean diferentes.
                    pero ¿que pasa si hay otra reserva del mismo inquilino en las nuevas fechas?
                    tremendo quilombo
                    no se deberian poder cambiar las fechas de una reserva y punto
                */
                await _repo.ActualizarAsync(reserva);
            }
            else
            {
                reserva.IdUsuarioReservador = userId.Value;
                await _repo.CrearAsync(reserva);
            }
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario([FromQuery] string? desde, [FromQuery] string? hasta, [FromRoute] int id = 0, [FromQuery] int idInq = 0, [FromQuery] int idInm = 0)
    {
        Reserva? reserva = new Reserva();
        if (desde != null && hasta != null)
        {
            var desdeArr = desde.Split("-");
            var hastaArr = hasta.Split("-");

            reserva.FechaInicio = new DateTime(int.Parse(desdeArr[0]), int.Parse(desdeArr[1]), int.Parse(desdeArr[2]));
            reserva.FechaFin = new DateTime(int.Parse(hastaArr[0]), int.Parse(hastaArr[1]), int.Parse(hastaArr[2]));
        }

        if (idInm > 0)
        {
            ViewBag.inmueble = await _repoInmueble.ObtenerPorIdAsync(idInm);
        }

        if (id > 0)
        {
            reserva = await _repo.ObtenerPorIdAsync(id);
            ViewBag.IdInmueble = reserva?.IdInmueble;
        }
        else
        {
            ViewBag.IdInmueble = idInm;
        }

        if (idInq > 0) 
            ViewBag.inquilino = await _repoInquilino.ObtenerPorIdAsync(idInq);
        
        ViewBag.IdInquilino = idInq;
        ViewBag.id = id;
        ViewBag.desde = desde;
        ViewBag.hasta = hasta;
        ViewBag.linkActivo = "reservas";
        ViewBag.MensajeError = TempData["MensajeError"];

        return View(reserva);
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();

        if (!await _repo.EliminarAsync(id))
            TempData["MensajeError"] = "No se pudo borrar la reserva";
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Renovar([FromRoute] int id)
    {
        Reserva? reserva = await _repo.ObtenerPorIdAsync(id);
        if (reserva == null)
            return NotFound();
            
        Reserva reservaNueva = new()
        {
            Id = 0,
            IdInmueble = reserva.IdInmueble,
            IdInquilino = reserva.IdInquilino,
            FechaInicio = reserva.FechaFin!.Value.AddDays(1),
            Monto = reserva.Monto
        };

        ViewBag.IdInquilino = reserva.IdInquilino;
        ViewBag.IdInmueble = reserva.IdInmueble;
        ViewBag.linkActivo = "reservas";
        /*
            tengo que limpiar el ModelState con 'ModelState.Clear()' porque
            el input con asp-for="Id" me lo renderizaba con el valor del parametro id del Action
            en lugar de usar el valor 0 con el creo el objeto
        */
        ModelState.Clear();
        return View(nameof(Formulario), reservaNueva);
    }

    [HttpGet]
    public async Task<IActionResult> Multa([FromRoute] long id, [FromServices] IInmobiliariaService inmobiliariaService)
    {
        if (id <= 0)
            return BadRequest();

        Reserva? reserva = await _repo.ObtenerPorIdAsync(id);

        if (reserva == null)
            return NotFound();

        return Json(await inmobiliariaService.GetMulta(reserva));
    }

    [HttpGet]
    public async Task<IActionResult> PorFechas(ReservasPorFechasViewModel vm, int pagina = 1, int cantidadPaginado = 10)
    {
        int cantidadReservas = 0;

        if (vm.Desde.HasValue && vm.Hasta.HasValue)
        {
            vm.Reservas =  (await _repo.ListarReservas(pagina, cantidadPaginado, null, vm.Desde.Value.ToString("yyyy-MM-dd"), vm.Hasta.Value.ToString("yyyy-MM-dd"))).ToList();
            cantidadReservas =  await _repo.ContarReservas(null, vm.Desde.Value.ToString("yyyy-MM-dd"), vm.Hasta.Value.ToString("yyyy-MM-dd"));
        }

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadReservas / cantidadPaginado);
        ViewBag.cantidadPaginado = cantidadPaginado;
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.linkActivo = "informes";

        ViewBag.MensajeError = vm.Reservas.Count == 0 && vm.Desde.HasValue && vm.Hasta.HasValue ? "No se encontraron resultados" : "";

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}