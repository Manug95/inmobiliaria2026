import { agregarClases, getElementById, mostrarMensaje , mostrarPregunta, removerClases, aFechaLocal } from "./frontUtils.js";

document.addEventListener("DOMContentLoaded", _ => {
  mostrarMensaje(false, null);
  let modalMulta;

  document.querySelectorAll("td .bi-trash")?.forEach(i => {
    i.addEventListener("click", e => {
      const idFila = e.target.id.split("-")[1];
      getElementById("form_eliminar").action = `/Reserva/Eliminar/${idFila}`;
      mostrarPregunta(null);
    });
  });
  
  document.querySelectorAll("td .bi-file-earmark-text")?.forEach(i => {
    i.addEventListener("click", async e => {
      const idFila = e.target.id.split("-")[1];
      try {
        const respuesta = await fetch(`/Reserva/Buscar/${idFila}`);
        if (respuesta.ok) {
          const reserva = await respuesta.json();
          mostrarModalDetalle(reserva);
        }
      } catch (error) {
        mostrarMensaje(false, "No se pudieron cargar los datos");
      }
    });
  });

  document.querySelectorAll("td .bi-file-earmark-x")?.forEach(i => {
    i.addEventListener("click", async e => {
      const idFila = e.target.id.split("-")[1];
      try {
        const respuesta = await fetch(`/Reserva/Multa/${idFila}`);
        if (respuesta.ok) {
          const datos = await respuesta.json();
          agregarDatosModalMulta(datos);
          if (modalMulta == undefined) {//console.log(getElementById('modal-multa'));
            modalMulta = new bootstrap.Modal(getElementById('modal-multa'), {});
          }

          modalMulta.show();
        }
      } catch (error) {
        mostrarMensaje(false, "No se pudieron cargar los datos de la multa");
      }
    });
  });
  
});

function agregarDatosModalMulta(datos) {
  getElementById("mensaje_total_dias").textContent = `${datos.totalDiasReserva} días`;
  getElementById("mensaje_cant_dias_reservados").textContent = `${datos.cantidadDiasReservados} días`;
  getElementById("mensaje_importe_total").textContent = `$ ${datos.importeTotalReserva}`;
  getElementById("mensaje_pagado").textContent = `$ ${datos.pagado}`;
  getElementById("mensaje_deuda").textContent = `$ ${datos.deuda}`;
  getElementById("mensaje_multa").textContent = `$ ${datos.importe}`;

  getElementById("enlacePagarMulta").href = `/Pago/Formulario?reservaId=${datos.reservaId}&&multa=${datos.importe}`;
}

function mostrarModalDetalle(reserva) {
  const bodyDetalle = getElementById("body-detalle");
  const bodyMensaje = getElementById("body-mensaje");
  if (reserva !== null) {
    removerClases(bodyDetalle, "d-none");
    agregarClases(bodyDetalle, "d-block");
    agregarClases(bodyMensaje, "d-none");
    getElementById("nro").textContent = reserva.id;
    getElementById("propietario").textContent = `${reserva.inmueble.duenio.apellido}, ${reserva.inmueble.duenio.nombre}`;
    getElementById("direccion").textContent = `${reserva.inmueble.calle} ${reserva.inmueble.nroCalle}`;
    getElementById("detalle_tipo").textContent = reserva.inmueble.tipo.tipo;
    getElementById("inquilino").textContent = `${reserva.inquilino.apellido}, ${reserva.inquilino.nombre}`;
    getElementById("fIni").textContent = aFechaLocal(reserva.fechaInicio);
    getElementById("fFin").textContent = aFechaLocal(reserva.fechaFin);
    getElementById("monto").textContent = `$ ${reserva.monto}`;
    getElementById("fTerm").textContent = reserva.fechaTerminado ? aFechaLocal(reserva.fechaTerminado) : " - ";
    const spanReservador = getElementById("reservador");
    if (spanReservador != undefined)
      spanReservador.textContent = `Cod: ${reserva.usuarioReservador.id} - ${reserva.usuarioReservador.apellido}, ${reserva.usuarioReservador.nombre}`;
    const spanTerminador = getElementById("terminador");
    if (spanTerminador != undefined)
      spanTerminador.textContent = reserva.idUsuarioTerminador ? `Cod: ${reserva.usuarioTerminador.id} - ${reserva.usuarioTerminador.apellido}, ${reserva.usuarioTerminador.nombre}` : " - ";
  } else {
    removerClases(bodyMensaje, "d-none");
    agregarClases(bodyMensaje, "d-block");
    agregarClases(bodyDetalle, "d-none");
  }
  const myModal = new bootstrap.Modal(getElementById('modal_detalle_reserva'), {});
  myModal.show();
}