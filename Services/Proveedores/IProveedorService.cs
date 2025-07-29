using IntelliSoftAPIV2.Dtos.Proveedores;

namespace IntelliSoftAPIV2.Services.Proveedores

{
    public interface IProveedorService
    {
       Task<List<ProveedorResponseDto>> ObtenerTodosAsync();
       Task<ProveedorResponseDto?> ObtenerPorIdAsync(int id);
       Task<ServiceResult<string>> CrearAsync(ProveedorCreateDto dto);
       Task<ServiceResult<string>> ActualizarAsync(int id, ProveedorUpdateDto dto);
       Task<ServiceResult<string>> EliminarAsync(int id);
    }
}
