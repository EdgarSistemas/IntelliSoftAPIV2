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

            return ServiceResult<string>.CreateSuccess(user.Id, "Usuario registrado correctamente");
        }

        // LOGIN 
        public async Task<object> Login(string email, string password)
        {
            // Login
            var usuario = await _userManager.FindByEmailAsync(email);
            if (usuario == null || !await _userManager.CheckPasswordAsync(usuario, password))
                return ServiceResult<string>.Failure("Credenciales Inválidas");

            var roles = await _userManager.GetRolesAsync(usuario);
            var token = _tokenService.CreateToken(usuario, roles);

            return new { token };
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
                FechaRegistro = user.fecha_registro,
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
                    Appellidos = user.Apellidos,
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
                    return ServiceResult<RegisterDto?>.CreateSuccess(null, "El usuario ya existe pero no es anónimo");

                var dtoExistente = new RegisterDto
                {
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Email = user.Email,
                    Password = "",
                    Rol = "anonimo"
                };

                return ServiceResult<RegisterDto?>.CreateSuccess(dtoExistente, "Usuario anónimo existente");
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

            string contrasenaGenerada = Guid.NewGuid().ToString("N").Substring(0, 8) + "!";

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

            return ServiceResult<RegisterDto?>.CreateSuccess(dtoNuevo, "Usuario anónimo creado correctamente");
        }

        // ACTUALIZAR USUARIO
        public async Task<ServiceResult<string>> UpdateUserAsync(UpdateUserDto dto, ClaimsPrincipal userClaims)
        {
            // Buscar al usuario
            var user = await _userManager.GetUserAsync(userClaims);
            if (user == null)
                return ServiceResult<string>.Failure("Usuario no encontrado");

            // Validar que el nuevo email (si viene) no esté en uso
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var emailExists = await _userManager.FindByEmailAsync(dto.Email);
                if (emailExists != null)
                    return ServiceResult<string>.Failure("El email proporcionado ya está en uso");

                user.Email = dto.Email;
                user.UserName = dto.Email;               // Mantener email = username
            }

            // Actualizar campos simples
            if (!string.IsNullOrWhiteSpace(dto.Nombre)) user.Nombre = dto.Nombre;
            if (!string.IsNullOrWhiteSpace(dto.Apellidos)) user.Apellidos = dto.Apellidos;
            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;

            // Gestionar cambio de contraseña (si se solicita)
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    return ServiceResult<string>.Failure("Debe proporcionar la contraseña actual para cambiarla");

                var passwordCheck = await _userManager.CheckPasswordAsync(user, dto.CurrentPassword);
                if (!passwordCheck)
                    return ServiceResult<string>.Failure("La contraseña actual es incorrecta");

                var resultCambioPass = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!resultCambioPass.Succeeded)
                    return ServiceResult<string>.Failure(resultCambioPass.Errors.FirstOrDefault()?.Description);
            }


            // Persistir los cambios
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return ServiceResult<string>.Failure(updateResult.Errors.FirstOrDefault()?.Description);

            return ServiceResult<string>.CreateSuccess(user.Id, "Usuario actualizado correctamente");
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
