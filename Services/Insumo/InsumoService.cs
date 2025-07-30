using IntelliSoftAPI.Models;
using IntelliSoftAPIV2.Dtos.Insumo;
using IntelliSoftAPIV2.Dtos.Insumos;
using IntelliSoftAPIV2.Dtos.Unidades;
using Microsoft.EntityFrameworkCore;

namespace IntelliSoftAPIV2.Services.Insumo
{
    public class InsumoService
    {
        private readonly AppDbContext _context;

        public InsumoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InsumoDto>> GetAll()
        {
            return await _context.TbInsumos
                .Include(i => i.Unidad)
                .Select(i => new InsumoDto
                {
                    IdInsumo = i.IdInsumo,
                    Nombre = i.Nombre,
                    Descripcion = i.Descripcion,
                    UnidadId = i.UnidadId,
                    Estatus = i.Estatus,
                    Unidad = new UnidadDto
                    {
                        IdUnidad = i.Unidad.IdUnidad,
                        Nombre = i.Unidad.Nombre,
                        Simbolo = i.Unidad.Simbolo,
                        Descripcion = i.Unidad.Descripcion,
                        Estatus = i.Unidad.Estatus
                    },
                    Existencias = _context.TbInventarioInsumos
                        .Where(inv => inv.InsumoId == i.IdInsumo)
                        .OrderByDescending(inv => inv.Fecha)
                        .Select(inv => inv.Existencias)
                        .FirstOrDefault(),
                    PrecioPromedio = _context.TbInventarioInsumos
                        .Where(inv => inv.InsumoId == i.IdInsumo)
                        .OrderByDescending(inv => inv.Fecha)
                        .Select(inv => (decimal?)inv.Promedio)
                        .FirstOrDefault()
                }).ToListAsync();
        }

        public async Task<InsumoDto?> GetById(int id)
        {
            var i = await _context.TbInsumos
                .Include(i => i.Unidad)
                .FirstOrDefaultAsync(x => x.IdInsumo == id);

            if (i == null) return null;

            var inventario = await _context.TbInventarioInsumos
                .Where(inv => inv.InsumoId == i.IdInsumo)
                .OrderByDescending(inv => inv.Fecha)
                .FirstOrDefaultAsync();

            return new InsumoDto
            {
                IdInsumo = i.IdInsumo,
                Nombre = i.Nombre,
                Descripcion = i.Descripcion,
                UnidadId = i.UnidadId,
                Estatus = i.Estatus,
                Unidad = new UnidadDto
                {
                    IdUnidad = i.Unidad.IdUnidad,
                    Nombre = i.Unidad.Nombre,
                    Simbolo = i.Unidad.Simbolo,
                    Descripcion = i.Unidad.Descripcion,
                    Estatus = i.Unidad.Estatus
                },
                Existencias = inventario?.Existencias,
                PrecioPromedio = inventario?.Promedio
            };
        }

        public async Task<ServiceResult<InsumoDto>> CreateAsync(InsumoCreateDto dto)
        {
            var insumo = new TbInsumo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                UnidadId = dto.UnidadId,
                Estatus = 1
            };

            _context.TbInsumos.Add(insumo);
            await _context.SaveChangesAsync();

            // Se carga la unidad relacionada
            var unidad = await _context.TbCatalogoUnidads.FindAsync(dto.UnidadId);

            var result = new InsumoDto
            {
                IdInsumo = insumo.IdInsumo,
                Nombre = insumo.Nombre,
                Descripcion = insumo.Descripcion,
                UnidadId = insumo.UnidadId,
                Estatus = insumo.Estatus,
                Unidad = unidad == null ? null : new UnidadDto
                {
                    IdUnidad = unidad.IdUnidad,
                    Nombre = unidad.Nombre,
                    Simbolo = unidad.Simbolo,
                    Descripcion = unidad.Descripcion,
                    Estatus = unidad.Estatus
                }
            };

            return ServiceResult<InsumoDto>.CreateSuccess(result, "Insumo creado correctamente");
        }

        public async Task<ServiceResult<InsumoDto>> UpdateAsync(int id, InsumoUpdateDto dto)
        {
            var insumo = await _context.TbInsumos
                .Include(i => i.Unidad)
                .FirstOrDefaultAsync(i => i.IdInsumo == id);

            if (insumo == null)
                return ServiceResult<InsumoDto>.Failure("Insumo no encontrado");

            insumo.Nombre = dto.Nombre;
            insumo.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();

            return ServiceResult<InsumoDto>.CreateSuccess(new InsumoDto
            {
                IdInsumo = insumo.IdInsumo,
                Nombre = insumo.Nombre,
                Descripcion = insumo.Descripcion,
                UnidadId = insumo.UnidadId,
                Estatus = insumo.Estatus,
                Unidad = new UnidadDto
                {
                    IdUnidad = insumo.Unidad.IdUnidad,
                    Nombre = insumo.Unidad.Nombre,
                    Simbolo = insumo.Unidad.Simbolo,
                    Descripcion = insumo.Unidad.Descripcion,
                    Estatus = insumo.Unidad.Estatus
                }
            }, "Insumo actualizado correctamente");
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var insumo = await _context.TbInsumos.FindAsync(id);
            if (insumo == null) return false;

            insumo.Estatus = 0;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
