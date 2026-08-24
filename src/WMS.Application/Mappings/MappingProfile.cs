using AutoMapper;
using WMS.Application.DTOs.Inbound;
using WMS.Application.DTOs.Inventory;
using WMS.Application.DTOs.MasterData;
using WMS.Application.DTOs.Outbound;
using WMS.Application.DTOs.Reports;
using WMS.Application.DTOs.Security;
using WMS.Domain.Entities.Inbound;
using WMS.Domain.Entities.Inventory;
using WMS.Domain.Entities.MasterData;
using WMS.Domain.Entities.Outbound;
using WMS.Domain.Entities.Security;

namespace WMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Master Data
        CreateMap<Item, ItemDto>();
        CreateMap<CreateItemDto, Item>();
        CreateMap<UpdateItemDto, Item>();

        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CreateWarehouseDto, Warehouse>();
        CreateMap<UpdateWarehouseDto, Warehouse>();

        CreateMap<Location, LocationDto>()
            .ForMember(d => d.WarehouseName, o => o.MapFrom(s => s.Warehouse != null ? s.Warehouse.WarehouseName : string.Empty));
        CreateMap<CreateLocationDto, Location>();

        CreateMap<Vendor, VendorDto>();
        CreateMap<CreateVendorDto, Vendor>();
        CreateMap<UpdateVendorDto, Vendor>();

        CreateMap<UnitOfMeasure, UOMDto>();
        CreateMap<CreateUOMDto, UnitOfMeasure>();
        CreateMap<UpdateUOMDto, UnitOfMeasure>();

        // Inbound
        CreateMap<PurchaseOrder, PurchaseOrderDto>()
            .ForMember(d => d.VendorName, o => o.MapFrom(s => s.Vendor != null ? s.Vendor.VendorName : string.Empty));
        CreateMap<PurchaseOrderDetail, PODetailDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty));
        CreateMap<CreatePODto, PurchaseOrder>();
        CreateMap<CreatePODetailDto, PurchaseOrderDetail>();

        CreateMap<GoodsReceiptNote, GRNDto>()
            .ForMember(d => d.PONumber, o => o.MapFrom(s => s.PurchaseOrder != null ? s.PurchaseOrder.PONumber : string.Empty));
        CreateMap<GoodsReceiptNoteDetail, GRNDetailDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.PutAwayLocationCode, o => o.MapFrom(s => s.PutAwayLocation != null ? s.PutAwayLocation.LocationCode : null));
        CreateMap<CreateGRNDto, GoodsReceiptNote>();
        CreateMap<CreateGRNDetailDto, GoodsReceiptNoteDetail>();

        // Inventory
        CreateMap<StockBalance, StockBalanceDto>()
            .ForMember(d => d.ItemCode, o => o.MapFrom(s => s.Item != null ? s.Item.ItemCode : string.Empty))
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.Location != null ? s.Location.LocationCode : string.Empty))
            .ForMember(d => d.WarehouseName, o => o.MapFrom(s => s.Location != null && s.Location.Warehouse != null ? s.Location.Warehouse.WarehouseName : string.Empty));

        CreateMap<StockLedger, StockLedgerDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.Location != null ? s.Location.LocationCode : string.Empty));

        CreateMap<StockAdjustment, StockAdjustmentDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.Location != null ? s.Location.LocationCode : string.Empty));

        CreateMap<StockTransfer, StockTransferDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.FromLocationCode, o => o.MapFrom(s => s.FromLocation != null ? s.FromLocation.LocationCode : string.Empty))
            .ForMember(d => d.ToLocationCode, o => o.MapFrom(s => s.ToLocation != null ? s.ToLocation.LocationCode : string.Empty));

        CreateMap<CycleCount, CycleCountDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.Location != null ? s.Location.LocationCode : string.Empty));

        // Outbound
        CreateMap<SalesOrder, SalesOrderDto>();
        CreateMap<SalesOrderDetail, SODetailDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty));
        CreateMap<CreateSODto, SalesOrder>();
        CreateMap<CreateSODetailDto, SalesOrderDetail>();

        CreateMap<PickingList, PickingListDto>()
            .ForMember(d => d.SONumber, o => o.MapFrom(s => s.SalesOrder != null ? s.SalesOrder.SONumber : string.Empty));
        CreateMap<PickingListDetail, PickingDetailDto>()
            .ForMember(d => d.ItemName, o => o.MapFrom(s => s.Item != null ? s.Item.ItemName : string.Empty))
            .ForMember(d => d.LocationCode, o => o.MapFrom(s => s.Location != null ? s.Location.LocationCode : string.Empty));

        CreateMap<Packing, PackingDto>()
            .ForMember(d => d.SONumber, o => o.MapFrom(s => s.SalesOrder != null ? s.SalesOrder.SONumber : string.Empty));

        CreateMap<Shipment, ShipmentDto>()
            .ForMember(d => d.PackNumber, o => o.MapFrom(s => s.Packing != null ? s.Packing.PackNumber : string.Empty));

        // Security
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(d => d.PasswordHash, o => o.Ignore());
    }
}
