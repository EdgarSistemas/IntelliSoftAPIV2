using IntelliSoftAPIV2.Configuration;
using IntelliSoftAPIV2.Dtos.Users;
using IntelliSoftAPIV2.Models;
using iText.Commons.Actions.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IntelliSoftAPIV2.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            TokenService tokenService,
            EmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _emailService = emailService;
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
            var users = await _userManager.Users.Where(u => u.estatus != false).ToListAsync();
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

            // Cambiar el estatus a 0 (borrado lógico)
            user.estatus = false;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }


        public async Task<ServiceResult<RegisterDto?>> ObtenerOCrearAnonimoPorEmail(string email, string nombre, string apellidos)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // Si ya existe
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var rol = roles.FirstOrDefault();

                if (rol != "anonimo")
                    return ServiceResult<RegisterDto?>.CreateSuccess(null, "El usuario ya existe pero no es anónimo");

                var dtoExistente = new RegisterDto
                {
                    Id = user.Id,
                    Nombre = user.Nombre,
                    Apellidos = user.Apellidos,
                    Email = user.Email,
                    Password = "", // No se recupera la contraseña aquí
                    Rol = "anonimo"
                };

                return ServiceResult<RegisterDto?>.CreateSuccess(dtoExistente, "Usuario anónimo existente");
            }

            // Crear nuevo usuario anónimo
            string contrasenaGenerada = GenerarContrasenaSegura(8);

            var nuevoUsuario = new ApplicationUser
            {
                Nombre = nombre,
                Apellidos = apellidos,
                Email = email,
                UserName = email,
                EmailConfirmed = true,
                ContrasenaGenerada = contrasenaGenerada // Campo personalizado que debes haber agregado
            };

            var result = await _userManager.CreateAsync(nuevoUsuario, contrasenaGenerada);
            if (!result.Succeeded)
                return ServiceResult<RegisterDto?>.Failure("Error al crear el usuario anónimo");

            // Asignar rol anónimo
            if (!await _roleManager.RoleExistsAsync("anonimo"))
                await _roleManager.CreateAsync(new IdentityRole("anonimo"));

            await _userManager.AddToRoleAsync(nuevoUsuario, "anonimo");

            // Guardar campo personalizado en la base (si no se guardó con CreateAsync)
            await _userManager.UpdateAsync(nuevoUsuario);

            var dtoNuevo = new RegisterDto
            {
                Id = nuevoUsuario.Id,
                Nombre = nuevoUsuario.Nombre,
                Apellidos = nuevoUsuario.Apellidos,
                Email = nuevoUsuario.Email,
                Password = "", // Se enviará más adelante
                Rol = "anonimo"
            };

            return ServiceResult<RegisterDto?>.CreateSuccess(dtoNuevo, "Usuario anónimo creado correctamente");
        }

        private static string GenerarContrasenaSegura(int longitud = 8)
        {
            if (longitud < 6) longitud = 6;

            var random = new Random();
            const string mayusculas = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string minusculas = "abcdefghijklmnopqrstuvwxyz";
            const string numeros = "0123456789";
            const string simbolos = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            // Garantizar al menos uno de cada tipo
            var passwordChars = new List<char>
            {
                mayusculas[random.Next(mayusculas.Length)],
                minusculas[random.Next(minusculas.Length)],
                numeros[random.Next(numeros.Length)],
                simbolos[random.Next(simbolos.Length)]
            };

            // Rellenar el resto con todos los tipos
            string todos = mayusculas + minusculas + numeros + simbolos;
            for (int i = passwordChars.Count; i < longitud; i++)
                passwordChars.Add(todos[random.Next(todos.Length)]);

            // Mezclar los caracteres
            passwordChars = passwordChars.OrderBy(x => random.Next()).ToList();

            return new string(passwordChars.ToArray());
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

        // Editar usuario
        public async Task<RegisterDto?> EditUser(EditDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null) return null;
            // Actualizar propiedades del usuario
            user.Email = dto.Email;
            user.UserName = dto.Email;
            // Actualizar contraseña si se proporciona
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!resetResult.Succeeded) return null;
            }
            // Guardar cambios
            var updateResult = await _userManager.UpdateAsync(user);

            return new RegisterDto { Id = user.Id, Nombre = user.Nombre, Apellidos = user.Apellidos, Email = user.Email };
        }

        public async Task<(bool Exito, string Mensaje)> ActualizarContrasenaAsync(string email)
        {
                var usuario = await _userManager.FindByEmailAsync(email);
                if (usuario == null)
                    return (false, "Usuario no encontrado");

                var nuevaContrasena = GenerarContrasenaSegura(8);
                await _userManager.RemovePasswordAsync(usuario);
                await _userManager.AddPasswordAsync(usuario, nuevaContrasena);

            string asunto = "Actualización de contraseña";
                string cuerpoHtml = $@"
                                    <p>Hola {usuario.Nombre},</p>
                                    <p>Tu contraseña ha sido restablecida. Tu nueva contraseña es:</p>
                                    <p style='font-size:18px; font-weight:bold; color:#2c3e50;'>{nuevaContrasena}</p>
                                    <p>Por favor cámbiala al iniciar sesión.</p>
                                    <br>
                                    <p>Saludos,<br>Equipo de soporte</p>
                                ";

                await _emailService.EnviarCorreoAsync(email, asunto, cuerpoHtml, null, null);

                return (true, "Contraseña actualizada y enviada al correo");
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
