using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Unidades;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Unidad
{
    public class UnidadService
    {
        private readonly AppDbContext _context;

        public UnidadService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<UnidadDto>> GetAll()
        {
            return await _context.TbCatalogoUnidads
                .Select(u => new UnidadDto
                {
                    IdUnidad = u.IdUnidad,
                    Nombre = u.Nombre,
                    Simbolo = u.Simbolo,
                    Descripcion = u.Descripcion,
                    Estatus = u.Estatus
                }).ToListAsync();
        }

        public async Task<UnidadDto?> GetById(int id)
        {
            var unidad = await _context.TbCatalogoUnidads.FindAsync(id);
            if (unidad == null) return null;

            return new UnidadDto
            {
                IdUnidad = unidad.IdUnidad,
                Nombre = unidad.Nombre,
                Simbolo = unidad.Simbolo,
                Descripcion = unidad.Descripcion,
                Estatus = unidad.Estatus
            };
        }

        public async Task<ServiceResult<UnidadDto>> CreateUnidadAsync(UnidadCreateDto dto)
        {
            var unidad = new TbCatalogoUnidad
            {
                Nombre = dto.Nombre,
                Simbolo = dto.Simbolo,
                Descripcion = dto.Descripcion,
                Estatus = 1
            };

            _context.TbCatalogoUnidads.Add(unidad);
            await _context.SaveChangesAsync();

            var result = new UnidadDto
            {
                IdUnidad = unidad.IdUnidad,
                Nombre = unidad.Nombre,
                Simbolo = unidad.Simbolo,
                Descripcion = unidad.Descripcion,
                Estatus = unidad.Estatus
            };

            return ServiceResult<UnidadDto>.CreateSuccess(result, "Unidad creada correctamente");
        }

        public async Task<ServiceResult<UnidadDto>> UpdateUnidadAsync(int id, UnidadCreateDto dto)
        {
            var unidad = await _context.TbCatalogoUnidads.FindAsync(id);
            if (unidad == null)
                return ServiceResult<UnidadDto>.Failure("Unidad no encontrada");

            unidad.Nombre = dto.Nombre;
            unidad.Simbolo = dto.Simbolo;
            unidad.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            var result = new UnidadDto
            {
                IdUnidad = unidad.IdUnidad,
                Nombre = unidad.Nombre,
                Simbolo = unidad.Simbolo,
                Descripcion = unidad.Descripcion,
                Estatus = unidad.Estatus
            };

            return ServiceResult<UnidadDto>.CreateSuccess(result, "Unidad actualizada correctamente");
        }

        public async Task<bool> DeleteUnidadAsync(int id)
        {
            var unidad = await _context.TbCatalogoUnidads.FindAsync(id);
            if (unidad == null) return false;

            unidad.Estatus = 0;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
