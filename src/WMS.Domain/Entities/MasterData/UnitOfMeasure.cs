using WMS.Domain.Common;

namespace WMS.Domain.Entities.MasterData;

public class UnitOfMeasure : BaseEntity
{
    public string UOMCode { get; set; } = string.Empty;
    public string UOMName { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; } = 1;
    public bool IsActive { get; set; } = true;
}
