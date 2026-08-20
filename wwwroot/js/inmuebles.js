import { getElementById, mostrarMensaje, mostrarPregunta } from "./frontUtils.js";


document.addEventListener("DOMContentLoaded", () => {
  const DETALLES = [];

  mostrarMensaje(false, null);
  
  document.querySelectorAll(".bi-trash")?.forEach(i => {
    i.addEventListener("click", e => {
      const idFila = e.target.id.split("-")[1];
      getElementById("form_eliminar").action = `/Inmueble/Eliminar/${idFila}`;
      mostrarPregunta(null);
    });
  });

  getElementById("filtro-disponible").addEventListener("change", e => {
    const select = e.target;

    document.querySelectorAll(".page-link").forEach(a => {
      if (!a.classList.contains("disabled")) {
        const queryParams = a.href.split("?")[1].split("&");
        const offset = queryParams.filter(q => q.includes("offset"))[0];
        if (offset !== undefined) {
          const offsetValue = offset.split("=")[1];
          a.href = `/Inmueble?disp=${select.value}&limit=10&offset=${offsetValue}`;
        }
      }
    });

  });

  getElementById("consultar-btn").addEventListener("click", e => {
    const a = e.target;
    const propietario = getElementById("propietario").value;
    const disponibilidad = getElementById("filtro-disponible").value;

    a.href = `/Inmueble?limit=10&offset=1`;
    if (disponibilidad !== undefined && disponibilidad.trim().length > 0) a.href += `&disp=${disponibilidad}`;
    if (propietario !== undefined && propietario.trim().length > 0) a.href += `&prop=${propietario}`;

  });
});