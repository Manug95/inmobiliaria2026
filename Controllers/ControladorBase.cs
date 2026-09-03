using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace inmobiliaria2026.Controllers;

public class ControladorBase : Controller
{
    protected string ModelStateError(ModelStateDictionary modelState)
    {
        string errorMsg = "";
        foreach (var estado in modelState)
        {
            var campo = estado.Key;
            foreach (var error in estado.Value.Errors)
            {
                errorMsg += $"{error.ErrorMessage}</br>";
            }
        }
        return errorMsg;
    }
}