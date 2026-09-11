using inmobiliaria2026.Models.ViewModels;

namespace inmobiliaria2026.Models;

public class Usuario : CredencialesBase
{
    public int Id { get; set; }
    public string? Rol { get; set; }
    public string? Avatar { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public bool Activo { get; set; }

    public override string ToString()
    {
        return $"Usuario ({Id}) - {Apellido}, {Nombre}";
    }

    public static Usuario Parse(UsuarioViewModel vm)
    {
        return new()
        {
            Id = vm.Id,
            Email = vm.Email,
            Password = vm.Password,
            Rol = vm.Rol,
            Nombre = vm.Nombre,
            Apellido = vm.Apellido,
            Avatar = vm.Avatar
        };
    }
}