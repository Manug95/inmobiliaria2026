using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models;

public class Propietario
{
    public int Id { get; set; }

    [StringLength(50, ErrorMessage = "El máximo de caracteres es 50")]
    [Required(ErrorMessage="El nombre es requerido")]
    public string? Nombre { get; set; } //el ? significa que la variable puede ser null

    [StringLength(50, ErrorMessage = "El máximo de caracteres es 50")]
    [Required(ErrorMessage = "El apellido es requerido")]
    public string? Apellido { get; set; }

    [StringLength(13, ErrorMessage = "El máximo de caracteres es 13")]
    [Required(ErrorMessage = "El DNI es requerido")]
    public string? Dni { get; set; }

    [StringLength(25, ErrorMessage = "El máximo de caracteres es 25")]
    [Required(ErrorMessage = "El teléfono es requerido")]
    [Display(Name = "Teléfono")]
    public string? Telefono { get; set; }

    [StringLength(100, ErrorMessage = "El máximo de caracteres es 100")]
    [Required(ErrorMessage = "El e-mail es requerido")]
    [EmailAddress(ErrorMessage = "El valor ingresado NO es un EMAIL")]
    [Display(Name = "E-Mail")]
    public string? Email { get; set; }

    public bool? Activo { get; set; }

    public Propietario() { }

    public Propietario(string nombre, string apellido, string dni, string telefono, string email)
    {
        Nombre = nombre;
        Apellido = apellido;
        Dni = dni;
        Telefono = telefono;
        Email = email;
    }

    public Propietario(int id, string nombre, string apellido, string dni, string telefono, string email)
    {
        Id = id;
        Nombre = nombre;
        Apellido = apellido;
        Dni = dni;
        Telefono = telefono;
        Email = email;
    }

    public override string ToString()
        => $"{Apellido}, {Nombre} - {Dni}";

}