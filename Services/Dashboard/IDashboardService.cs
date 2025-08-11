using IntelliSoftAPIV2.Dtos.Dashboard;

namespace IntelliSoftAPIV2.Services.Dashboard
{
    public interface IDashboardService
    {
        Task<List<PedidosEstatusDto>> ObtenerPedidosEstatusAsync(DateTime? from = null, DateTime? to = null);
        Task<List<MesValorDto>> ObtenerIngresosMensualesAsync(DateTime? from = null, DateTime? to = null);
        Task<List<MesValorDto>> ObtenerPedidosMensualesAsync(DateTime? from = null, DateTime? to = null);
        Task<List<ProductoMasVendidoDto>> ObtenerTopProductosAsync(string metric, DateTime? from, DateTime? to, int take);
        Task<List<ClienteTopDto>> ObtenerClientesTopAsync(DateTime? from, DateTime? to, int take);
        Task<List<ProductoOpinionDto>> ObtenerProductosMejorCalificadosAsync(DateTime? from = null, DateTime? to = null, int take = 10);
        Task<List<DistribucionOpinionDto>> ObtenerDistribucionOpinionesAsync(DateTime? from = null, DateTime? to = null);
        Task<ConversionDto> ObtenerConversionAsync(DateTime? from = null, DateTime? to = null);
    }
}
