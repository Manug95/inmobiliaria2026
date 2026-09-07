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
    public async Task<IActionResult> Guardar([FromForm] Pago pago, [FromServices] IReservaRepository repoReserva)
    {
        if (ModelState.IsValid)
        {
            if (pago.Id > 0)
            {
                await _repo.ActualizarAsync(new Pago(){ Id = pago.Id, Concepto = pago.Concepto });
            }
            else
            {
                // ¿deberia comprobar que el importe ingresado no sea mayor que lo que se debe pagar?
                if (pago.ReservaId <= 0)
                {
                    TempData["MensajeError"] = "No se especificó la reserva del pago";
                    return RedirectToAction(nameof(Index), "Reserva");
                }
                
                Reserva? reserva = await repoReserva.ObtenerPorIdAsync(pago.ReservaId);
                decimal sumaImportes = await _repo.SumarImportes(pago.ReservaId);

                if (reserva != null)
                {
                    decimal importeTotalReserva = 
                        reserva.Monto!.Value * (reserva.FechaFin!.Value - reserva.FechaInicio!.Value).Days;

                    if (pago.Importe > (importeTotalReserva - sumaImportes))
                    {
                        TempData["MensajeError"] = "El importe ingresado es mayor que el precio total de la reserva";
                        return RedirectToAction(nameof(Formulario), new { reservaId = pago.ReservaId });
                    }
                    else
                        await _repo.CrearAsync(pago);
                }
                // 👆aca termina la logica para comprobar lo de la pregunta anterior👆

                // necesito alguna forma de saber si el pago nuevo es de una multa
                // si es asi, calculo el valor de esta
                // luego la comparo con el valor del importe del formulario
                // si son iguales creo el pago
                // y tambien tengo que darle la fecha actual a la FechaTerminado de la reserva

                // await _repo.CrearAsync(pago);
            }
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
            TempData["MensajeError"] = "No se pudo eliminar el pago";

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Formulario([FromQuery] bool multa, [FromQuery] long reservaId, [FromServices] IReservaRepository repoReserva, [FromRoute] long id)
    {
        if (reservaId < 0 || id < 0)
            return BadRequest();

        Pago? pago = null;

        if (reservaId > 0 && id == 0) // es un CREATE
        {
            Reserva? reserva = await repoReserva.ObtenerPorIdAsync(reservaId);
            decimal sumaImportes = await _repo.SumarImportes(reservaId);

            if (reserva != null)
            {
                decimal importeTotalReserva = 
                    reserva.Monto!.Value * (reserva.FechaFin!.Value - reserva.FechaInicio!.Value).Days;

                decimal importe;

                ViewBag.deuda = sumaImportes > 0 
                    ? importeTotalReserva - sumaImportes 
                    : importeTotalReserva;

                importe = sumaImportes > 0 
                    ? importeTotalReserva - sumaImportes 
                    : importeTotalReserva * (reserva.Inmueble!.Senia / 100);

                pago = new(){ Id = id, ReservaId = reservaId, Importe = importe, Fecha = DateTime.Now };
            }

            // falta calcular el valor de la multa cuando 'multa' es true
            // calculo el valor de la multa y paso ese valor a ViewBag en lugar del monto

            /*
                
            */
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
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(pago);
    }
}