using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Proveedores;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Proveedores
{
    public class ProveedorService : IProveedorService
    {
        private readonly AppDbContext _context;

        public ProveedorService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<ProveedorResponseDto>> ObtenerTodosAsync()
        {
            return await _context.TbProveedors
                .Where(p => p.Estatus == 1)
                .Select(p => new ProveedorResponseDto
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    Telefono = p.Telefono,
                    Contacto = p.Contacto,
                    CorreoElectronico = p.CorreoElectronico,
                    DescripcionServicio = p.DescripcionServicio,
                    Estatus = p.Estatus
                })
                .ToListAsync();
        }

        public async Task<ProveedorResponseDto?> ObtenerPorIdAsync(int id)
        {
            var p = await _context.TbProveedors.FindAsync(id);
            if (p == null) return null;

            return new ProveedorResponseDto
            {
                IdProveedor = p.IdProveedor,
                Nombre = p.Nombre,
                Telefono = p.Telefono,
                Contacto = p.Contacto,
                CorreoElectronico = p.CorreoElectronico,
                DescripcionServicio = p.DescripcionServicio,
                Estatus = p.Estatus
            };
        }

        public async Task<ServiceResult<string>> CrearAsync(ProveedorCreateDto dto)
        {
            // Validar si el email ya existe
            if (await ExisteProveedorConEmailAsync(dto.CorreoElectronico))
            {
                return ServiceResult<string>.Failure("Ya existe un proveedor con este email.");
            }

            //Crear proveedor
            var proveedor = new TbProveedor
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Contacto = dto.Contacto,
                CorreoElectronico = dto.CorreoElectronico,
                DescripcionServicio = dto.DescripcionServicio,
                Estatus = dto.Estatus
            };

            _context.TbProveedors.Add(proveedor);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Proveedor creado exitosamente");
        }

        public async Task<ServiceResult<string>> ActualizarAsync(int id, ProveedorUpdateDto dto)
        {
            var proveedor = await _context.TbProveedors.FindAsync(id);
            if (proveedor == null)
            {
                return ServiceResult<string>.Failure("Proveedor no encontrado");

            }

            proveedor.Nombre = dto.Nombre;
            proveedor.Telefono = dto.Telefono;
            proveedor.Contacto = dto.Contacto;
            proveedor.CorreoElectronico = dto.CorreoElectronico;
            proveedor.DescripcionServicio = dto.DescripcionServicio;

            await _context.SaveChangesAsync();
            return ServiceResult<string>.CreateSuccess("Proveedor actualizado correctamente");
        }

        public async Task<ServiceResult<string>> EliminarAsync(int id)
        {
            var proveedor = await _context.TbProveedors.FindAsync(id);
            if (proveedor == null)
                return ServiceResult<string>.Failure("Proveedor no encontrado");

            proveedor.Estatus = 0;
            _context.TbProveedors.Update(proveedor);
            await _context.SaveChangesAsync();

            return ServiceResult<string>.CreateSuccess("Proveedor eliminado correctamente");
        }

        //Metodo para validar el correo
        public async Task<bool> ExisteProveedorConEmailAsync(string email)
        {
            return await _context.TbProveedors.AnyAsync(p => p.CorreoElectronico == email);
        }
    }
}
