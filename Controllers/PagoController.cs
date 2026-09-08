using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

public class PagoController : ControladorBase
{
    private readonly IPagoRepository _repo;

    public PagoController(IPagoRepository repo)
    {
        _repo = repo;
    }

    public async Task<IActionResult> Index([FromQuery] long reservaId = 0, [FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        List<Pago> pagos = await _repo.ListarPagos(pagina, cantidadPaginado, reservaId);
        long cantidadPagos = await _repo.ContarPagos(reservaId);

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadPagos / cantidadPaginado);
        ViewBag.cantidadPaginado = cantidadPaginado;
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.reservaId = reservaId;
        ViewBag.linkActivo = "pagos";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(pagos);
    }

    public async Task<IActionResult> Buscar([FromRoute] int id)
    {
        Pago? pago = await _repo.ObtenerPorIdAsync(id);
        return Json(new { pago });
    }

    [HttpPost]
    public async Task<IActionResult> Guardar(
        [FromForm] Pago pago, 
        [FromForm] bool esMulta, 
        [FromServices] IReservaRepository repoReserva,
        [FromServices] IInmobiliariaService inmobiliariaService
    )
    {
        if (ModelState.IsValid)
        {
            if (pago.Id > 0)
            {
                await _repo.ActualizarAsync(new Pago(){ Id = pago.Id, Concepto = pago.Concepto });
            }
            else
            {
                if (pago.ReservaId <= 0)
                {
                    TempData["MensajeError"] = "No se especificó la reserva del pago";
                    return RedirectToAction(nameof(Index), "Reserva");
                }
                
                Reserva? reserva = await repoReserva.ObtenerPorIdAsync(pago.ReservaId);

                if (reserva != null)
                {
                    if (esMulta)
                    {
                        var multa = await inmobiliariaService.GetMulta(reserva);

                        if (pago.Importe != multa.Importe)
                        {
                            TempData["MensajeError"] = $"El pago de la multa debe ser de $ {multa.Importe:F2}.";
                            return RedirectToAction(nameof(Formulario), new { reservaId = pago.ReservaId, multa = multa.Importe });
                        }

                        reserva.FechaTerminado = DateTime.Today;
                        await repoReserva.ActualizarAsync(reserva);
                    }
                    else
                    {
                        decimal sumaImportes = await _repo.SumarImportes(pago.ReservaId);
                        decimal importeTotalReserva = 
                            reserva.Monto!.Value * (reserva.FechaFin!.Value - reserva.FechaInicio!.Value).Days;
                        var pagoInicial = importeTotalReserva * (reserva.Inmueble!.Senia / 100);
                        
                        if (sumaImportes == 0 && pago.Importe != pagoInicial)
                        {
                            TempData["MensajeError"] = $"El importe del primer pago debe ser $ ${pagoInicial} correspondiente al porcentaje de la seña del inmueble";
                            return RedirectToAction(nameof(Formulario), new { reservaId = pago.ReservaId });
                        }

                        if (pago.Importe > (importeTotalReserva - sumaImportes))
                        {
                            TempData["MensajeError"] = "El importe ingresado es mayor que el precio total de la reserva";
                            return RedirectToAction(nameof(Formulario), new { reservaId = pago.ReservaId });
                        }
                    }
                    
                    await _repo.CrearAsync(pago);
                }
                else
                {
                    TempData["MensajeError"] = "No se puede realizar el pago a una reserva que no existe";
                    return RedirectToAction("Reserva");
                }
            }
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
            var parametros = new RouteValueDictionary();
            if (pago.ReservaId > 0)
                parametros["reservaId"] = pago.ReservaId;
            if (pago.Id > 0)
                parametros["id"] = pago.Id;
            if (esMulta)
                parametros["multa"] = pago.Importe;
            return RedirectToAction(nameof(Formulario), parametros);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar([FromRoute] int id)
    {
        if (id <= 0)
            return BadRequest();

        if (!await _repo.EliminarAsync(id))
            TempData["MensajeError"] = "No se pudo eliminar el pago";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario([FromQuery] long reservaId, [FromRoute] long id, [FromServices] IReservaRepository repoReserva, [FromQuery] decimal? multa)
    {
        if (reservaId < 0 || id < 0)
            return BadRequest();

        Pago? pago;
        bool esPagoDeMulta = false;

        if (reservaId > 0 && id == 0) // es un CREATE
        {
            Reserva? reserva = await repoReserva.ObtenerPorIdAsync(reservaId);

            if (reserva != null)
            {
                decimal importe;

                if (multa.HasValue)
                {
                    ViewBag.deuda = multa.Value;
                    importe = multa.Value;
                    esPagoDeMulta = true;
                }
                else
                {
                    decimal sumaImportes = await _repo.SumarImportes(reservaId);
                    decimal importeTotalReserva = 
                        reserva.Monto!.Value * (reserva.FechaFin!.Value - reserva.FechaInicio!.Value).Days;
                    
                    ViewBag.deuda = sumaImportes > 0 
                    ? (importeTotalReserva - sumaImportes).ToString("F2") 
                    : importeTotalReserva.ToString("F2");

                    importe = sumaImportes > 0 
                    ? importeTotalReserva - sumaImportes 
                    : importeTotalReserva * (reserva.Inmueble!.Senia / 100);
                }

                pago = new(){ Id = id, ReservaId = reservaId, Importe = importe, Fecha = DateTime.Now };
            }
            else
            {
                TempData["MensajeError"] = "No se puede realizar el pago a una reserva que no existe";
                return RedirectToAction("Reserva");
            }
        }
        else if (id > 0 && reservaId == 0) // es un UPDATE
        {
            pago = await _repo.ObtenerPorIdAsync(id);
        }
        else
        {
            return BadRequest(); // si (id > 0 && reservaId > 0) o (id == 0 && reservaId == 0) esta mal la peticion
        }

        ViewBag.linkActivo = "pagos";
        ViewBag.esPagoDeMulta = esPagoDeMulta;
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(pago);
    }
}