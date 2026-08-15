import { getElementById, getFormInputValue, createElement } from "./frontUtils.js";
import {
  resetValidationInputStyle,
  resetValidationErrorMessage,
  setInvalidInputStyle,
  setValidInputStyle,
  setValidationErrorMessage,
  validarFormSelect,
  validarNombreTipoInmueble,
  validarDescripcionTipoInmueble,
  validarCalle,
  validarCupo,
  validarLatitud,
  validarLongitud,
  validarNroCalle,
  validarPrecio,
  validarSenia
} from "./validaciones.js";


document.addEventListener("DOMContentLoaded", () => {
  let modalTipoInmueble = null;
  const form = getElementById("form-inmueble");
  let tipoInmueble = null;

  form?.addEventListener("submit", (e) => {
    e.preventDefault();

    const formValues = getFormValues();

    if (validarFormulario(formValues)) {
      if (tipoInmueble !== null) {
        getElementById("NuevoTipo").value = tipoInmueble.Tipo;
        getElementById("NuevoTipoDescripcion").value = tipoInmueble.Descripcion;
      }

      setTimeout((form) => { form.submit(); }, 500, form);
    }

    return;

  });

  const TipoInmueble = getElementById("form-tipoInmueble");
  
  TipoInmueble?.addEventListener("submit", (e) => {
    e.preventDefault();
    const formValues = getTipoInmuebleFormValues();
    
    if (validarTipoInmuebleFormulario(formValues)) {
      tipoInmueble = {
        Id: 0,
        Tipo: formValues.tipo,
        Descripcion: formValues.descripcion
      };
      const option = createElement("option", { value: tipoInmueble.Id, content: tipoInmueble.Tipo });
      getElementById("tipo_inm").appendChild(option);

      modalTipoInmueble.hide();
    } 
    
    return;

  });

  getElementById("add_tipo").addEventListener("click", e => {
    getElementById("tipo").value = "";
    getElementById("descripcion").value = "";

    resetTipoInmuebleValidationStatus();

    if (modalTipoInmueble === null) {
      const modalForm = getElementById('modal_formulario_tipoInmueble');
      modalTipoInmueble = new bootstrap.Modal(modalForm, {});
    }
    modalTipoInmueble.show();
  });

  getElementById("buscador")
  .addEventListener("input", debounce((e) => {
      buscar(e.target.value);
    }, 500)
  );

  const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
  const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

});

function getFormValues() {
  return {
    calle: getFormInputValue("calle"),
    nro: getFormInputValue("nro"),
    tipo_inm: getFormInputValue("tipo_inm"),
    uso: getFormInputValue("uso"),
    cupo: getFormInputValue("cupo"),
    precio: getFormInputValue("precio"),
    senia: getFormInputValue("senia"),
    latitud: getFormInputValue("latitud"),
    longitud: getFormInputValue("longitud"),
    propietario: getFormInputValue("propietario"),
  };
}

/**
 * @param {*} values 
 * @returns {Boolean}
 */
function validarFormulario(values) {
  const mapaValidador = new Map([
    ["calle", validarCalle],
    ["nro", validarNroCalle],
    ["tipo_inm", validarFormSelect],
    ["uso", validarFormSelect],
    ["cupo", validarCupo],
    ["precio", validarPrecio],
    ["senia", validarSenia],
    ["latitud", validarLatitud],
    ["longitud", validarLongitud],
    ["propietario", validarFormSelect]
  ]);

  const esValido = Object.keys(values)
    .map(v => {
      const result = mapaValidador.get(v)(values[v]);
      if (result !== undefined) { setInvalidInputStyle(v); setValidationErrorMessage(`tpe_${v}`, result.errorMessage); }
      else { setValidInputStyle(v); resetValidationErrorMessage(`tpe_${v}`); }
      return result === undefined;
    })
    .every(v => v);

  return esValido;
}

// PARA EL FORM DEL TIPO DE INMUEBLE

/**
 * @param {*} values 
 * @returns {Boolean}
 */
function validarTipoInmuebleFormulario(values) {
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

function resetTipoInmuebleValidationStatus() {
  resetValidationInputStyle("tipo");
  resetValidationInputStyle("descripcion");

  resetValidationErrorMessage("tpe_tipo");
  resetValidationErrorMessage("tpe_descripcion");
}

function getTipoInmuebleFormValues() {
  return {
    tipo: getFormInputValue("tipo"),
    descripcion: getFormInputValue("descripcion")
  };
}

// PARA LA BUSQUEDA DE LOS PROPIETARIOS

function debounce(fn, delay) {
  let timer;
  return function(...args) {
    clearTimeout(timer);
    timer = setTimeout(() => fn.apply(this, args), delay);
  }
}

async function buscar(valor) {
  if (!valor) return;

  try {
    const response = await fetch(`/Propietario/Listar?nomApe=${valor}&orderBy=Apellido&order=ASC`);
    const data = await response.json();

    const select = getElementById("propietario");
    select.innerHTML = "";

    data.datos.forEach(item => {
      const option = createElement("option", { value: item.id, content: `${item.apellido}, ${item.nombre} - ${item.dni}` });
      select.appendChild(option);
    });

  } catch (error) {
    console.error("Error en la búsqueda:", error);
  }
}