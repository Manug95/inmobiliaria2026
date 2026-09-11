using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class CredencialesBase
{
    [StringLength(100, ErrorMessage = "El máximo de caracteres es 100")]
    [Required(ErrorMessage = "El e-mail es requerido")]
    [EmailAddress(ErrorMessage = "El valor ingresado NO es un e-mail")]
    [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "NO es un e-mail válido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Falta la contraseña")]
    [StringLength(60, ErrorMessage = "La contraseña de debe tener entre 3 y 60 caracteres", MinimumLength = 3)]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}