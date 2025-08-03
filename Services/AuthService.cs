using IntelliSoftAPIV2.Dtos.Users;
using IntelliSoftAPIV2.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IntelliSoftAPIV2.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        // REGISTRAR UN NUEVO USUARIO 
        public async Task<ServiceResult<string>> RegisterAsync(RegisterDto dto)
        {
            // Verificar si el usuario ya existe
            var userExists = await _userManager.FindByEmailAsync(dto.Email);
            if (userExists != null)
                return ServiceResult<string>.Failure("El usuario ya existe");

            // Crear nuevo usuario
            var user = new ApplicationUser
            {
                Nombre = dto.Nombre,
                Apellidos = dto.Apellidos,
                Email = dto.Email,
                UserName = dto.Email
            };

            // Crear usuario
            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return ServiceResult<string>.Failure(createResult.Errors.FirstOrDefault()?.Description);
            var userId = user.Id;
            
            // Verificar y crear rol si no existe
            if (!await _roleManager.RoleExistsAsync(dto.Rol))
            {
                await _roleManager.CreateAsync(new IdentityRole(dto.Rol));
            }

            // Asignar rol al usuario
            var roleResult = await _userManager.AddToRoleAsync(user, dto.Rol);
            if (!roleResult.Succeeded)
            {
                // Opcional: eliminar usuario si falla la asignación de rol
                await _userManager.DeleteAsync(user);
                return ServiceResult<string>.Failure("No se pudo asignar el rol al usuario");
            }

            return ServiceResult<string>.SuccessResult(userId);

        }

        // LOGIN 
        public async Task<ServiceResult<object>> Login(string email, string password)
        {
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, password))
                return ServiceResult<object>.Failure("Credenciales inválidas");

            var roles = await _userManager.GetRolesAsync(usuario);
            var token = _tokenService.CreateToken(usuario, roles);

            return ServiceResult<object>.SuccessResult(new { token });
        }
        public class ServiceResult<T>
        {
            public bool Success { get; set; }
            public T? Data { get; set; }
            public string? Message { get; set; }

            public static ServiceResult<T> SuccessResult(T data) => new() { Success = true, Data = data };
            public static ServiceResult<T> Failure(string message) => new() { Success = false, Message = message };
        }


        // Detalles del usuario 
        public async Task<object> GetUserDetail(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new
            {
                Email = user.Email,
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                PhoneNumber = user.PhoneNumber,
                Rol = roles.FirstOrDefault()
            };
        }

        // Listar todos los usuarios 
        public async Task<List<object>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(new
                {
                    Id = user.Id,
                    Email = user.Email,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Rol = roles.FirstOrDefault()
                });
            }
            return result;
        }

        // Corrección en el método DeleteUser para resolver el error CS1503
        public async Task<bool> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            // Eliminar roles primero
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                foreach (var role in roles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<ServiceResult<RegisterDto?>> ObtenerOCrearAnonimoPorEmail(string email, string nombre, string apellidos)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // si ya existe
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var rol = roles.FirstOrDefault();

                if (rol != "anonimo")
                    return ServiceResult<RegisterDto?>.Failure( "El usuario ya existe pero no es anónimo");

                var dtoExistente = new RegisterDto
                {
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Email = user.Email,
                    Password = "",
                    Rol = "anonimo"
                };

                return ServiceResult<RegisterDto?>.SuccessResult(dtoExistente);
            }

            // crear nuevo usuario anónimo
            var nuevoUsuario = new ApplicationUser
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Email = email,
                UserName = email,
                EmailConfirmed = true
            };

            string baseContrasena = Guid.NewGuid().ToString("N").Substring(0, 8);
            string contrasenaConMayuscula = char.ToUpper(baseContrasena[0]) + baseContrasena.Substring(1);
            string contrasenaGenerada = contrasenaConMayuscula + "!";

            var result = await _userManager.CreateAsync(nuevoUsuario, contrasenaGenerada);
            if (!result.Succeeded)
                return ServiceResult<RegisterDto?>.Failure("Error al crear el usuario anónimo");

            // Asignar rol anónimo
            if (!await _roleManager.RoleExistsAsync("anonimo"))
                await _roleManager.CreateAsync(new IdentityRole("anonimo"));

            await _userManager.AddToRoleAsync(nuevoUsuario, "anonimo");

            var dtoNuevo = new RegisterDto
            {
                Nombre = nuevoUsuario.Nombre,
                Apellidos = nuevoUsuario.Apellidos,
                Email = nuevoUsuario.Email,
                Password = "",
                Rol = "anonimo"
            };

            return ServiceResult<RegisterDto?>.SuccessResult(dtoNuevo);
        }
    }



    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }

        // Renamed the static method to avoid conflict with the property 'Success'
        public static ServiceResult<T> CreateSuccess(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static ServiceResult<T> Failure(string message)
            => new() { Success = false, Message = message };
    }
}
