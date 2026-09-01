import { agregarClases, getElementById, mostrarMensaje , mostrarPregunta, removerClases} from "./frontUtils.js";

document.addEventListener("DOMContentLoaded", _ => {
  mostrarMensaje(false, null);

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
        const reserva = await respuesta.json();
        mostrarModalDetalle(reserva);
      } catch (error) {
        mostrarMensaje(false, "No se pudieron cargar los datos");
      }
    });
  });
  
});

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
    getElementById("fIni").textContent = aFechaLocal(reserva.fechaInicio.split("T")[0]);
    getElementById("fFin").textContent = aFechaLocal(reserva.fechaFin.split("T")[0]);
    getElementById("monto").textContent = `$ ${reserva.monto}`;
    getElementById("fTerm").textContent = reserva.fechaTerminado ? aFechaLocal(reserva.fechaTerminado?.split("T")[0]) : " - ";
  } else {
    removerClases(bodyMensaje, "d-none");
    agregarClases(bodyMensaje, "d-block");
    agregarClases(bodyDetalle, "d-none");
  }
  const myModal = new bootstrap.Modal(getElementById('modal_detalle_reserva'), {});
  myModal.show();
}

function aFechaLocal(fecha) {
  if (!fecha) return " - ";
  return fecha.split("-").reverse().join("/");
}