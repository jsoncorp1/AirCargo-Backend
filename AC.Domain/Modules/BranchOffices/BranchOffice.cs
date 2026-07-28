using AC.Domain.Common;
using AC.Domain.Modules.OrderDeliveries;
using AC.Domain.Modules.Shipments;
using AC.Domain.Modules.Users;

namespace AC.Domain.Modules.BranchOffices;

public class BranchOffice: CoreEntity
{
    public string Code { get; set; } = string.Empty;

    public BolivianDepartment BolivianDepartment { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Address { get; set; } = string.Empty;

    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string Phone { get; set; } = string.Empty;

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Shipment> OriginShipments { get; set; } = new List<Shipment>();
    public ICollection<Shipment> DestinationShipments { get; set; } = new List<Shipment>();
}