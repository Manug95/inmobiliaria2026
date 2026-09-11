using System.Security.Claims;
using System.Text.Json;
using inmobiliaria2026.Interfaces;
using inmobiliaria2026.Models;
using inmobiliaria2026.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliaria2026.Controllers;

public class UsuarioController(
    IUsuarioRepository usuarioRepository, 
    IConfiguration config, 
    IWebHostEnvironment env,
    IInmobiliariaService inmobiliariaService
    ) : ControladorBase
{
    private readonly IUsuarioRepository _repo = usuarioRepository;
    private readonly IConfiguration _config = config;
    private readonly IWebHostEnvironment _env = env;
    private readonly IInmobiliariaService _inmobiliaria = inmobiliariaService;

    [HttpGet]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Index([FromQuery] int pagina = 1, [FromQuery] int cantidadPaginado = 10)
    {
        List<Usuario> usuarios = await _repo.ListarAsync(cantidadPaginado, pagina);
        int cantidadUsuarios = await _repo.ContarUsuariosAsync();

        ViewBag.cantPag = Math.Ceiling((decimal)cantidadUsuarios / cantidadPaginado);
        ViewBag.cantidadPaginado = cantidadPaginado;
        ViewBag.paginaSiguiente = pagina + 1;
        ViewBag.paginaAnterior = pagina - 1;
        ViewBag.linkActivo = "usuarios";
        ViewBag.MensajeError = TempData["MensajeError"] as string;

        return View(UsuarioViewModel.ParseList(usuarios));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string returnUrl)
    {
        TempData["returnUrl"] = returnUrl;
        ViewBag.MensajeError = TempData["MensajeError"] as string;
        var loginViewModel = new LoginViewModel();
        string? email = TempData["Mail"] as string;
        if (!string.IsNullOrWhiteSpace(email))
            loginViewModel.Email = email;
        return View(loginViewModel);
    }

    [HttpGet]
    public async Task<ActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromForm] LoginViewModel login)
    {
        var returnUrl = string.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"]?.ToString();
        if (ModelState.IsValid)
        {
            if (login.Email == null || login.Password == null)
            {
                TempData["MensajeError"] = "No ingreso el e-mail o la contraseña";
                TempData["returnUrl"] = returnUrl;
                return RedirectToAction(nameof(Login));
            }

            string hashed = HashearPassword(login.Password);

            Usuario? user = await _repo.ObtenerPorEmailAsync(login.Email);
            if (user == null || user.Password != hashed)
            {
                TempData["MensajeError"] = "El email o la clave son correctos incorrecto(s)";
                TempData["Mail"] = login.Email;
                TempData["returnUrl"] = returnUrl;
                return RedirectToAction(nameof(Login));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Nombre + " " + user.Apellido),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Uri, user.Avatar ?? ""),
                new Claim(ClaimTypes.Role, user.Rol!),
                new Claim("id", user.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            TempData.Remove("returnUrl");

            return Redirect(returnUrl ?? "/");
        }
        else
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
            TempData["Mail"] = login.Email;
            // TempData["returnUrl"] = returnUrl;
            return RedirectToAction(nameof(Login), new { returnUrl });
        }
    }

    [HttpGet]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Create()
    {
        ViewBag.MensajeError = TempData["MensajeError"] as string;
        var json = TempData["diccionario"] as string;
        UsuarioViewModel vm = DeserializarDiccionarioUsuarioViewModel(json);
        return View(vm);
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Guardar([FromForm] UsuarioViewModel vm, [FromServices] IFileService fileService)
    {
        if (!ModelState.IsValid)
        {
            TempData["MensajeError"] = ModelStateError(ModelState);
            TempData["diccionario"] = SerializarDiccionarioUsuarioViewModel(vm);
            return RedirectToAction(nameof(Create));
        }

        if (string.IsNullOrEmpty(vm.Password))
        {
            TempData["MensajeError"] = "No ingresó la contraseña";
            return RedirectToAction(nameof(Create));
        }

        try
        {
            string hashed = HashearPassword(vm.Password);
            vm.Password = hashed;
            //var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
            vm.Id = await _repo.CrearAsync(Usuario.Parse(vm));
            if (vm.AvatarFile != null && vm.Id > 0)
            {
                // GuardarAvatarDelUsuario(vm);
                vm.Avatar = await fileService.GuardarAvatarDelUsuario(vm.AvatarFile, $"avatar_{vm.Id}");
                await _repo.ActualizarAsync(Usuario.Parse(vm));
            }
            return RedirectToAction(nameof(Index));
        }
        catch (Exception)
        {
            TempData["MensajeError"] = "No se pudo crear el usuario";
            return RedirectToAction(nameof(Create));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit([FromRoute] int id)
    {
        var userId = _inmobiliaria.GetUserId(User);
        if (!userId.HasValue)
            return Unauthorized();

        if (!User.IsInRole(Rol.ADMIN.ToString()) && userId.Value != id)
        {
            TempData["MensajeError"] = "No se pueden editar los datos de otro usuario";
            return RedirectToAction(nameof(Index), "Home");
        }

        ViewBag.MensajeError = TempData["MensajeError"] as string;
        Usuario? usuario = await _repo.ObtenerPorIdAsync(id);

        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario no existe";
            return RedirectToAction(nameof(Index));
        }

        return View(new EditUsuarioViewModel() { UsuarioViewModel = UsuarioViewModel.Parse(usuario) , CambiarContrasenia = new CambiarContraseniaViewModel() });
    }

    [HttpPost]
    public async Task<IActionResult> Actualizar([Bind(Prefix = nameof(UsuarioViewModel))] [FromForm] UsuarioViewModel vm)
    {
        // if (User.Claims.FirstOrDefault(c => c.Type == "id")?.Value == vm.Id.ToString())
        int? userId = _inmobiliaria.GetUserId(User);
        if (!userId.HasValue)
            return Unauthorized();
            
        if (userId.Value == vm.Id  || User.IsInRole(Rol.ADMIN.ToString()))
        {
            Usuario? usuario = await _repo.ObtenerPorIdAsync(vm.Id);

            if (usuario == null)
            {
                TempData["MensajeError"] = "El usuario no existe";
                return RedirectToAction(User.IsInRole(Rol.ADMIN.ToString()) ? nameof(Index) : "Home");
            }

            usuario.Nombre = vm.Nombre;
            usuario.Apellido = vm.Apellido;
            usuario.Rol = vm.Rol;

            await _repo.ActualizarAsync(usuario);

            //actualizo la claim que tiene el nombre y el apelldio del usuario
            if (userId.Value == vm.Id)
            {
                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    var claimNombreApellido = identity.FindFirst(ClaimTypes.Name);
                    if (claimNombreApellido != null)
                    {
                        identity.RemoveClaim(claimNombreApellido);
                        var newClaim = new Claim(ClaimTypes.Name, usuario.Nombre + " " + usuario.Apellido);
                        identity.AddClaim(newClaim);
                    }
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));
                }
            }
        }
        else
        {
            TempData["MensajeError"] = "No se pueden editar los datos de otro usuario";
            return RedirectToAction(nameof(Index), "Home");
        }
        
        return RedirectToAction(nameof(Edit), new { id = vm.Id });
    }

    [HttpPost]
    public async Task<IActionResult> CambiarContrasenia([Bind(Prefix = nameof(CambiarContraseniaViewModel))] [FromForm] CambiarContraseniaViewModel vm)
    {
        if (vm.Id <= 0)
        {
            TempData["MensajeError"] = "ID de usuario incorrecta";
            return RedirectToAction(nameof(Index), "Home");
        }

        var userId = _inmobiliaria.GetUserId(User);

        if (!userId.HasValue)
            return Unauthorized();

        if (!User.IsInRole(Rol.ADMIN.ToString()) && userId.Value != vm.Id)
        {
            TempData["MensajeError"] = "No se puede cambiar la constraseña de otro usuario";
            return RedirectToAction(nameof(Edit), new { id = vm.Id });
        }

        Usuario? usuario = await _repo.ObtenerPorIdAsync(vm.Id);

        if (usuario == null)
        {
            TempData["MensajeError"] = "El usuario no existe";
            return RedirectToAction(nameof(Index));
        }

        if (vm.PasswordNuevo != null)
        {
            if (vm.PasswordViejo != null && usuario.Password != HashearPassword(vm.PasswordViejo))
            {
                TempData["MensajeError"] = "La contraseña vieja es incorrecta";
                return RedirectToAction(nameof(Edit), new { id = vm.Id });
            }
            usuario.Password = HashearPassword(vm.PasswordNuevo);
        }
        else
        {
            TempData["MensajeError"] = "No cambió la contraseña porque no se ingresó una nueva";
            return RedirectToAction(nameof(Edit), new { id = vm.Id });
        }

        await _repo.ActualizarAsync(usuario);

        return RedirectToAction(nameof(Edit), new { id = vm.Id });
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarAvatar([Bind(Prefix = nameof(UsuarioViewModel))] [FromForm] UsuarioViewModel vm, [FromServices] IFileService fileService)
    {
        if (vm.Id <= 0)
        {
            TempData["MensajeError"] = "ID de usuario incorrecta";
            return RedirectToAction(nameof(Index), "Home");
        }

        if (vm.AvatarFile == null)
        {
            TempData["MensajeError"] = "No se subio ningún archivo de imagen";
            return RedirectToAction(nameof(Edit), new { id = vm.Id });
        }

        var userId = _inmobiliaria.GetUserId(User);

        if (!userId.HasValue)
            return Unauthorized();

        if (!User.IsInRole(Rol.ADMIN.ToString()) && userId.Value != vm.Id)
        {
            TempData["MensajeError"] = "No se puede cambiar el avatar de otro usuario";
            return RedirectToAction(nameof(Edit), new { id = vm.Id });
        }

        Usuario? usuario = await _repo.ObtenerPorIdAsync(vm.Id);

        if (usuario == null)
            return BadRequest();
        
        // BorrarAvatar(usuario.Id, usuario.Avatar ?? "");
        // GuardarAvatarDelUsuario(vm);
        fileService.BorrarAvatar(usuario.Id, usuario.Avatar ?? "");
        usuario.Avatar = await fileService.GuardarAvatarDelUsuario(vm.AvatarFile, $"avatar_{vm.Id}");
        await _repo.ActualizarAsync(usuario);

        //actualizo la claim del avatar del usuario si el usuario actualiza el propio
        if (userId.Value == vm.Id)
        {
            var identity = User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var claimAvatar = identity.FindFirst(ClaimTypes.Uri);
                if (claimAvatar != null)
                {
                    identity.RemoveClaim(claimAvatar);
                    var newClaim = new Claim(ClaimTypes.Uri, usuario.Avatar!);
                    identity.AddClaim(newClaim);
                }
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
        }
        
        return RedirectToAction(nameof(Edit), new { id = vm.Id });
    }

    [HttpPost]
    public async Task<IActionResult> EliminarAvatar([FromRoute] int id, [FromServices] IFileService fileService)
    {
        if (id <= 0)
            return BadRequest();
        
        try
        {
            Usuario? usuario = await _repo.ObtenerPorIdAsync(id);
            if (usuario != null)
            {
                // BorrarAvatar(id, usuario.Avatar ?? "");
                fileService.BorrarAvatar(id, usuario.Avatar ?? "");
                usuario.Avatar = null;
                await _repo.ActualizarAsync(usuario);
            }
        }
        catch
        {
            TempData["MensajeError"] = "No se pudo eliminar el avatar";
        }

        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost]
    [Authorize(Policy = "ADMIN")]
    public async Task<IActionResult> Eliminar([FromRoute] int id, [FromServices] IFileService fileService)
    {
        if (id <= 0)
            return BadRequest();

        if (!await _repo.EliminarAsync(id))
            TempData["MensajeError"] = "No se pudo eliminar el usuario";

        string? avatar = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Uri)?.Value;
        fileService.BorrarAvatar(id, avatar ?? "");

        return RedirectToAction(nameof(Index));
    }

    private string HashearPassword(string password)
    {
        return Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: System.Text.Encoding.ASCII.GetBytes(_config["Salt"] ?? "sal"),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8))
        ;
    }

    private static string SerializarDiccionarioUsuarioViewModel(UsuarioViewModel vm)
    {
        var diccionario = new Dictionary<string, string>();
        diccionario["Nombre"] = vm.Nombre ?? "";
        diccionario["Apellido"] = vm.Apellido ?? "";
        diccionario["Email"] = vm.Email ?? "";
        diccionario["Password"] = vm.Password ?? "";
        diccionario["Rol"] = vm.Rol ?? "";
        return JsonSerializer.Serialize(diccionario);
    }

    private static UsuarioViewModel DeserializarDiccionarioUsuarioViewModel(string? json)
    {
        UsuarioViewModel vm = new ();
        if (json != null)
        {
            var diccionario = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (diccionario != null)
            {
                vm.Nombre = diccionario["Nombre"];
                vm.Apellido = diccionario["Apellido"];
                vm.Email = diccionario["Email"];
                vm.Password = diccionario["Password"];
                vm.Rol = diccionario["Rol"];
            }
        }
        return vm;
    }
}