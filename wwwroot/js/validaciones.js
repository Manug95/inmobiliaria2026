import { agregarClases, removerClases, getElementById } from "./frontUtils.js";

export function setValidInputStyle(id) {
  const input = getElementById(id);
  if (input !== null) {
    removerClases(input, "is-invalid");
    agregarClases(input, "is-valid");
  }
}

export function setInvalidInputStyle(id) {
  const input = getElementById(id);
  if (input !== null) {
    removerClases(input, "is-valid");
    agregarClases(input, "is-invalid");
  }
}

export function resetValidationInputStyle(id) {
  const input = getElementById(id);
  if (input !== null) {
    removerClases(input, "is-valid");
    removerClases(input, "is-invalid");
  }
}

export function setValidationErrorMessage(tooltipId, message) {
  const tooltip = getElementById(tooltipId);
  if (tooltip !== null) tooltip.innerText = message;
}

export function resetValidationErrorMessage(tooltipId) {
  const tooltip = getElementById(tooltipId);
  if (tooltip !== null) tooltip.innerText = "";
}

export function validarFormSelect(value) {
  if (value === "") {
    return { errorMessage: "El campo es requerido" };
  }
  return;
}

export function validarFecha(fecha) {
  if (!fecha) return false;

  let fechaSel = new Date(fecha);
  if (!fechaSel) return false;
  fechaSel = fechaSel.getTime();

  const fechaAct = new Date().getTime();
  if (fechaSel > fechaAct) return false;
  
  return true;
}

export function validarDNI(dni) {
  if (dni === null) return { errorMessage: "El DNI es requerido" };

  if (dni.length > 13) return { errorMessage: "El máximo de caracteres es 13" };

  return;
}

/**
 * @param {string} cadena 
 */
export function validarNombreApellido(cadena) {
  if (cadena === undefined || cadena === null) return { errorMessage: "El campo es requerido" };
  if (typeof cadena !== "string") return { errorMessage: "El campo de ser de texto" };
  if (cadena.trim() === "") return { errorMessage: "El campo es requerido" };
  if (cadena.length > 50) return { errorMessage: "El máximo de caracteres de de 50" };
  return;
}

/**
 * @param {string} cadena 
 * @returns {undefined | { errorMessage: string }} El mensaje de error si falla la validación
 */
export function validarTelefono(cadena) {
  if (cadena.trim().length == 0) return;
  if (cadena.length > 25) return { errorMessage: "El máximo de caracteres de de 25." };

  // const regExp = /^(\+54\s)?0?(\d{2,4})\s(15\s)?(\d{4}-?\d{4})|(\d{3}-?\d{4})|(\d{2}-?\d{4})$/;
  // if (!regExp.test(cadena)) return { errorMessage: "El teléfono ingresado NO es valido" };
  return;
}

export function validarEmail(cadena) {
  if (cadena.length > 100) return { errorMessage: "El máximo de caracteres de de 100" };
  const regExp = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/; // la ultima parte {2,} podria ser {2,4}
  if (!regExp.test(cadena)) return { errorMessage: "El EMAIL ingresado NO es valido" };
  return;
}

/**
 * @param {string} cadena 
 */
export function validarNombreTipoInmueble(cadena) {
  if (cadena === undefined || cadena === null) return { errorMessage: "El campo es requerido" };
  if (typeof cadena !== "string") return { errorMessage: "El campo de ser de texto" };
  if (cadena.trim() === "") return { errorMessage: "El campo es requerido" };
  if (cadena.length > 50) return { errorMessage: "El máximo de caracteres de de 50" };
  return;
}

export function validarDescripcionTipoInmueble(cadena) {
  if (cadena !== undefined && cadena !== null) {
    if (typeof cadena !== "string") return { errorMessage: "El campo de ser de texto" };
  }
  if (cadena.length > 255) return { errorMessage: "El máximo de caracteres de de 255" };
  return;
}

export function validarNroCalle(cadena) {
  if (cadena.length === 0) {
    return { errorMessage: "El número de calle esta vacío o es incorrecto" };
  }
  const num = parseInt(cadena);
  if (isNaN(num)) return { errorMessage: "No es un número" };
  if (num <= 0) {
    return { errorMessage: "El número de calle debe ser mayor a 0" };
  }
  if (num > 99999) {
    return { errorMessage: "El número de calle es demasiado grande" };
  }
  return;
}

export function validarCalle(cadena) {
  if (cadena.length === 0) {
    return { errorMessage: "La calle es obligatoria" };
  }
  if (cadena.length > 100) {
    return { errorMessage: "La calle no puede superar los 100 caracteres" };
  }
  return;
}

export function validarCupo(cadena, requrido = true) {
  if (cadena.length === 0) {
    if (!requrido) return;

    return { errorMessage: "El cupo del inmueble es obligatorio" };
  }
  if (isNaN(cadena)) {
    return { errorMessage: "El cupo debe ser un número" };
  }
  const cupo = parseInt(cadena, 10);

  if (!Number.isInteger(cupo) || (cupo < Number.parseFloat(cadena))) {
    return { errorMessage: "El cupo debe ser un número entero" };
  }
  if (cupo <= 0) {
    return { errorMessage: "El cupo debe ser mayor que 0" };
  }
  if (cupo > 100) {
    return { errorMessage: "El cupo no puede superar los 100" };
  }
  return;
}

export function validarPrecio(cadena, requrido = true) {
  if (cadena.length === 0) {
    if (!requrido) return;
    
    return { errorMessage: "El campo es obligatorio" };
  }
  let precioNum = parseFloat(cadena.replace(",", "."));
  if (isNaN(precioNum)) {
    return { errorMessage: "Debe ser un número" };
  }
  if (precioNum <= 0) {
    return { errorMessage: "Debe ser mayor que 0" };
  }
  return;
}

export function validarSenia(cadena) {
  if (cadena.length === 0) {
    return { errorMessage: "El porcentaje de la seña del inmueble es obligatorio" };
  }
  if (isNaN(cadena)) {
    return { errorMessage: "El debe ser un número" };
  }
  
  const senia = parseInt(cadena, 10);

  if (!Number.isInteger(senia) || (senia < Number.parseFloat(cadena))) {
    return { errorMessage: "El debe ser un número entero" };
  }
  if (senia <= 0) {
    return { errorMessage: "El debe ser mayor que 0" };
  }
  if (senia > 100) {
    return { errorMessage: "No puede se mayor que 100" };
  }
  return;
}

export function validarLatitud(cadena) {
  if (cadena.length === 0) {
    return;
  }
  let latNum = parseFloat(cadena);
  if (isNaN(latNum)) {
    return { errorMessage: "La latitud debe ser un número" };
  }
  if (latNum < -90 || latNum > 90) {
    return { errorMessage: "La latitud debe estar entre -90 y 90" };
  }
  return;
}

export function validarLongitud(cadena) {
  if (cadena.length === 0) {
    return;
  }
  let lngNum = parseFloat(cadena);
  if (isNaN(lngNum)) {
    return { errorMessage: "La longitud debe ser un número" };
  }
  if (lngNum < -180 || lngNum > 180) {
    return { errorMessage: "La longitud debe estar entre -180 y 180" };
  }
  return;
}

export function validarFechaDeInputDate(cadena) {
  if (!cadena) return { errorMessage: "Fecha incompleta o incorrecta" };
  return;
}

export function validarFechaInicioDelContrato(inico, fin, terminacion, esNuevoContrato) {
  const fechaIni = new Date(inico + "T00:00:00");
    
  const msFechaIni = fechaIni.getTime();
  const msFechaFIn = new Date(fin + "T00:00:00").getTime();

  if (msFechaIni > msFechaFIn) return { errorMessage: "F. inicio es mayor a la f. fin del contrato" };
  if (terminacion) {
    const fechaTerm = new Date(terminacion + "T00:00:00").getTime();
    if (msFechaIni > fechaTerm) return { errorMessage: "F. inicio es mayor a la f. terminación del contrato" };
  }

  return;
}

export function validarFechaFinDelContrato(inico, fin, terminacion) {
  const fechaIni = new Date(inico).getTime();
  const fechaFIn = new Date(fin).getTime();

  if (fechaFIn < fechaIni) return { errorMessage: "F. fin es menor a la f. incio del contrato" };
  if (terminacion) {
    const fechaTerm = new Date(terminacion).getTime();
    if (fechaFIn > fechaTerm) return { errorMessage: "F. fin es mayor a la f. terminación del contrato" };
  }

  return;
}

export function validarFechaTerminacionDelContrato(inico, fin, terminacion) {
  if (terminacion) {
    const fechaIni = new Date(inico).getTime();
    const fechaFin = new Date(fin).getTime();
    const fechaTerm = new Date(terminacion).getTime();

    if (fechaTerm < fechaIni) return { errorMessage: "¿puede un contrato cancelarse antes de que inicie?" };
    if (fechaTerm > fechaFin) return { errorMessage: "F. terminación es mayor a la f. fin del contrato" };
  }

  return;
}