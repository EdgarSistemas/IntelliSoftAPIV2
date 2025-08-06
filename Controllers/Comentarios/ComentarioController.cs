using IntelliSoftAPIV2.Dtos.Comentarios;
using IntelliSoftAPIV2.Services.Comentarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IntelliSoftAPIV2.Controllers.Comentarios
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComentarioController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;

        public ComentarioController(IComentarioService comentarioService)
        {
            _comentarioService = comentarioService;
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> Crear([FromBody] ComentarioCreateDto dto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errores = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage)
        //            .ToList();

        //        return BadRequest(new { errores });
        //    }

        //    await _comentarioService.CrearAsync(dto);
        //    return Ok(new { message = "Comentario creado correctamente" });
        //}


        //[HttpGet("getAll")]
        //public async Task<IActionResult> ObtenerTodos()
        //{
        //    var comentarios = await _comentarioService.ObtenerTodosAsync();
        //    return Ok(comentarios);
        //}

        //[Authorize(Roles = "admin")]
        //[HttpDelete("delete/{id}")]
        //public async Task<IActionResult> Eliminar(int id)
        //{
        //    try {
        //        var resultado = await _comentarioService.EliminarAsync(id);
        //        if (!resultado.Success)
        //            return NotFound(new { message = "Comentario no encontrado." });

        //        return Ok(new { message = "Comentario eliminado correctamente." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(new { message = ex.Message });
        //    }
           
        //}
    }
}
