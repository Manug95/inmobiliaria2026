import { agregarClases, getElementById, getFormInputValue, mostrarMensaje, mostrarPregunta, removerClases } from "./frontUtils.js";
import { 
  setInvalidInputStyle, 
  setValidInputStyle,
  resetValidationInputStyle ,
  setValidationErrorMessage,
  resetValidationErrorMessage,
  validarNombreTipoInmueble,
  validarDescripcionTipoInmueble
} from "./validaciones.js";


document.addEventListener("DOMContentLoaded", () => {

  const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
  const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

  getElementById("btn_add").addEventListener("click", e => {
    getElementById("tipo").value = "";
    getElementById("descripcion").value = "";
    getElementById("Id").value = 0;

    resetValidationStatus();

    const modalForm = getElementById('modal_formulario_tipoInmueble');
    const myModal = new bootstrap.Modal(modalForm, {});
    myModal.show();
  });

  document.querySelectorAll("td .bi-pencil-square").forEach(i => {
    i.addEventListener("click", e => {
      const idFIla = e.target.id.split("-")[1];
      const fila = getElementById(idFIla);
      const datosFila = getRawValues(fila);

      getElementById("tipo").value = datosFila.tipo;
      getElementById("Id").value = datosFila.id;

      resetValidationStatus();
      
      const myModal = new bootstrap.Modal(getElementById('modal_formulario_tipoInmueble'), {});
      myModal.show();
    });
  });

  document.querySelectorAll("td .bi-trash")?.forEach(i => {
    i.addEventListener("click", e => {
      const idFila = e.target.id.split("-")[1];

      getElementById("btn_si").href = `/TipoInmueble/Eliminar/${idFila}`;

      mostrarPregunta(null);
    });
  });

  mostrarMensaje(false, null);

  const form = getElementById("form-tipoInmueble");

  form?.addEventListener("submit", (e) => {
    e.preventDefault();
    const formValues = getFormValues();
    
    if (validarFormulario(formValues)) {
      // setTimeout((form) => { form.submit(); }, 500, form);
      form.submit();
    }

    return;

  });

});

function getFormValues() {
  return {
    tipo: getFormInputValue("tipo"),
    descripcion: getFormInputValue("descripcion")
  };
}

/**
 * @param {HTMLTableRowElement} raw 
 */
function getRawValues(raw) {
  const datos = raw.children;
  return {
    tipo: datos.item(0).innerText.trim(),
    descripcion: datos.item(1).innerText.trim(),
    id: raw.id
  };
}

/**
 * @param {*} values 
 * @returns {Boolean}
 */
function validarFormulario(values) {
  const mapaValidador = new Map([
    ["tipo", validarNombreTipoInmueble],
    ["descripcion", validarDescripcionTipoInmueble]
  ]);

  return Object.keys(values)
  .map(v => {
    const result = mapaValidador.get(v)(values[v]);
    if (result !== undefined) { setInvalidInputStyle(v); setValidationErrorMessage(`tpe_${v}`, result.errorMessage); } 
    else { setValidInputStyle(v); resetValidationErrorMessage(`tpe_${v}`); }
    return result === undefined;
  })
  .every(v => v);
}

function resetValidationStatus() {
    resetValidationInputStyle("tipo");
    resetValidationInputStyle("descripcion");

    resetValidationErrorMessage("tpe_tipo");
    resetValidationErrorMessage("tpe_descripcion");
}