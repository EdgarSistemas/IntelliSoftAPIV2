using IntelliSoftAPIV2.Dtos.Login;
using IntelliSoftAPIV2.Dtos.Users;
using IntelliSoftAPIV2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = result.Message,
                userId = result.Data
            });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.Login(dto.Email, dto.Password);
            if (result == null) return Unauthorized("Credenciales inválidas");
            return Ok(result);
        }

        [Authorize]
        [HttpGet("detail")]
        public async Task<IActionResult> GetUserDetail()
        {
            var result = await _authService.GetUserDetail(User);
            if (result == null) return NotFound("Usuario no encontrado");
            return Ok(result);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _authService.GetAllUsers();
            return Ok(users);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _authService.DeleteUser(id);
            if (!success) return NotFound("Usuario no encontrado");
            return Ok(new { message = "Usuario eliminado correctamente" });
        }

        [HttpPost("anonimo-verificar-o-crear")]
        public async Task<IActionResult> VerificarOCrearAnonimo([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest(new { message = "Email y Nombre son requeridos" });

            var result = await _authService.ObtenerOCrearAnonimoPorEmail(dto.Email, dto.Nombre, dto.Apellidos);

            return Ok(new
            {
                creado = result.Message.Contains("creado"),
                usuario = result.Data,
                message = result.Message
            });
        }
    }
}