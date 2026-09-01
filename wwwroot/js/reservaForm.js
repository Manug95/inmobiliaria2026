import { getElementById, getFormInputValue, createElement, mostrarMensaje } from "./frontUtils.js";
import {
  resetValidationErrorMessage,
  setInvalidInputStyle,
  setValidInputStyle,
  setValidationErrorMessage,
  validarFormSelect,
  validarPrecio,
  validarFechaDeInputDate,
  validarFechaInicioDelContrato,
  validarFechaFinDelContrato,
  validarFechaTerminacionDelContrato
} from "./validaciones.js";


document.addEventListener("DOMContentLoaded", () => {
  mostrarMensaje(false, null);

  const form = getElementById("form-reserva");
  form?.addEventListener("submit", (e) => {
    e.preventDefault();

    const formValues = getFormValues();

    if (validarFormulario(formValues)) {
      if (validarRangosDeFechas(formValues)) 
        form.submit();
    }

    return;
  });

  getElementById("buscador")
  ?.addEventListener("input", debounce((e) => {
      buscar(e.target.value);
    }, 500)
  );
});

function getFormValues() {
  const inputsValues = {
    IdInquilino: getFormInputValue("IdInquilino"),
    FechaInicio: getFormInputValue("FechaInicio"),
    FechaFin: getFormInputValue("FechaFin"),
    FechaTerminado: getFormInputValue("FechaTerminado"), //esto puede no estar
    Monto: getFormInputValue("Monto")
  };

  return inputsValues;
}

/**
 * @param {*} values 
 * @returns {Boolean}
 */
function validarFormulario(values) {
  const mapaValidador = new Map([
    ["FechaInicio", validarFechaDeInputDate],
    ["FechaFin", validarFechaDeInputDate],
    ["Monto", validarPrecio]
  ]);

  if (values["IdInquilino"] !== null) mapaValidador.set("IdInquilino", validarFormSelect);

  const esValido = Object.keys(values)
    .map(v => {
      const fnValidadora = mapaValidador.get(v);
      if (fnValidadora === undefined) return true;
      const result = fnValidadora(values[v]);
      if (result !== undefined) { setInvalidInputStyle(v); setValidationErrorMessage(`tpe_${v}`, result.errorMessage); }
      else { setValidInputStyle(v); resetValidationErrorMessage(`tpe_${v}`); }
      return result === undefined;
    })
    .every(v => v);

  return esValido;
}

function validarRangosDeFechas(formValues) {
  const mapaValidador = new Map([
    ["FechaInicio", validarFechaInicioDelContrato],
    ["FechaFin", validarFechaFinDelContrato]
  ]);
  if (formValues["FechaTerminado"] !== null) mapaValidador.set("FechaTerminado", validarFechaTerminacionDelContrato);

  const {FechaInicio, FechaFin, FechaTerminado} = formValues;
  const reservaId = +getElementById("Id").value;

  return Object.keys(formValues)
    .map(v => {
      const fnValidadora = mapaValidador.get(v);
      if (fnValidadora === undefined) return true;
      const result = fnValidadora(FechaInicio, FechaFin, FechaTerminado, reservaId === 0);
      if (result !== undefined) { setInvalidInputStyle(v); setValidationErrorMessage(`tpe_${v}`, result.errorMessage); }
      else { setValidInputStyle(v); resetValidationErrorMessage(`tpe_${v}`); }
      return result === undefined;
    })
    .every(v => v)
  ;
}

// PARA LA BUSQUEDA DE LOS INQUILINOS

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
    const response = await fetch(`/Inquilino/Listar?nomApe=${valor}&orderBy=Apellido&order=ASC`);
    const data = await response.json();

    const select = getElementById("IdInquilino");
    select.innerHTML = "";

    data.datos.forEach(item => {
      select.appendChild( createElement("option", { value: item.id, content: `${item.apellido}, ${item.nombre} - ${item.dni}` }) );
    });

  } catch (error) {
    console.error("Error en la búsqueda:", error);
  }
}