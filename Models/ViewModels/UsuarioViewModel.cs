using System.ComponentModel.DataAnnotations;

namespace inmobiliaria2026.Models.ViewModels;

public class UsuarioViewModel : CredencialesBase
{
    [Display(Name = "Código")]
    public int Id { get; set; }

    [Required(ErrorMessage = "El ROL es requerido")]
    [AllowedValues(["ADMIN", "EMPLEADO"], ErrorMessage = "ROL inválido")]
    [Display(Name = "ROL")]
    public string? Rol { get; set; }

    [DataType(DataType.ImageUrl)]
    [Url(ErrorMessage = "No es una URL válida")]
    public string? Avatar { get; set; }

    [DataType(DataType.Upload)]
    [MaxFileSize(2)] // máximo 2 MB
    public IFormFile? AvatarFile { get; set; }

    [StringLength(50, ErrorMessage = "El máximo de caracteres es 50")]
    // [Required(ErrorMessage="El nombre es requerido")]
    public string? Nombre { get; set; }

    [StringLength(50, ErrorMessage = "El máximo de caracteres es 50")]
    // [Required(ErrorMessage = "El apellido es requerido")]
    public string? Apellido { get; set; }

    public static UsuarioViewModel Parse(Usuario u)
    {
        return new()
        {
            Id = u.Id,
            Email = u.Email,
            Password = u.Password,
            Rol = u.Rol,
            Nombre = u.Nombre,
            Apellido = u.Apellido,
            Avatar = u.Avatar
        };
    }

    public static List<UsuarioViewModel> ParseList(List<Usuario> usuarios)
    {
        List<UsuarioViewModel> lista = [];
        foreach (var u in usuarios)
        {
            lista.Add(Parse(u));
        }
        return lista;
    }

    public override string ToString()
    {
        return $"{Apellido}, {Nombre}";
    }
}

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxSizeInMb;
    public MaxFileSizeAttribute(int maxSizeInMb)
    {
        _maxSizeInMb = maxSizeInMb;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var file = value as IFormFile;
        if (file != null && file.Length > _maxSizeInMb * 1024 * 1024)
        {
            return new ValidationResult($"El archivo no puede superar {_maxSizeInMb} MB.");
        }
        return ValidationResult.Success;
    }
}