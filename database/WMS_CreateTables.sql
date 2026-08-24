-- ============================================================
-- WMS (Warehouse Management System) - Database Schema
-- Server  : DESKTOP-RVR7FSH\SQLEXPRESS
-- Database: WMS_DB
-- ============================================================

USE WMS_DB;
GO

-- ============================================================
-- 1. MASTER DATA
-- ============================================================

-- UnitOfMeasure
CREATE TABLE UnitsOfMeasure (
    Id                UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    UOMCode           NVARCHAR(20)        NOT NULL,
    UOMName           NVARCHAR(100)       NOT NULL,
    ConversionFactor  DECIMAL(18,6)       NOT NULL DEFAULT 1,
    IsActive          BIT                 NOT NULL DEFAULT 1,
    IsDeleted         BIT                 NOT NULL DEFAULT 0,
    CreatedAt         DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy         NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt         DATETIME2           NULL,
    UpdatedBy         NVARCHAR(100)       NULL,
    CONSTRAINT PK_UnitsOfMeasure PRIMARY KEY (Id),
    CONSTRAINT UQ_UnitsOfMeasure_UOMCode UNIQUE (UOMCode)
);
GO

-- Items (Products)
CREATE TABLE Items (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ItemCode    NVARCHAR(50)        NOT NULL,
    ItemName    NVARCHAR(200)       NOT NULL,
    Description NVARCHAR(500)       NULL,
    UOM         NVARCHAR(20)        NOT NULL,
    Category    NVARCHAR(100)       NULL,
    MinStock    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    MaxStock    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsActive    BIT                 NOT NULL DEFAULT 1,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_Items PRIMARY KEY (Id),
    CONSTRAINT UQ_Items_ItemCode UNIQUE (ItemCode)
);
GO

-- Warehouses
CREATE TABLE Warehouses (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    WarehouseCode   NVARCHAR(20)        NOT NULL,
    WarehouseName   NVARCHAR(200)       NOT NULL,
    Address         NVARCHAR(500)       NULL,
    IsActive        BIT                 NOT NULL DEFAULT 1,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_Warehouses PRIMARY KEY (Id),
    CONSTRAINT UQ_Warehouses_WarehouseCode UNIQUE (WarehouseCode)
);
GO

-- Locations (Bins)
CREATE TABLE Locations (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    LocationCode    NVARCHAR(50)        NOT NULL,
    Aisle           NVARCHAR(20)        NULL,
    Rack            NVARCHAR(20)        NULL,
    Level           NVARCHAR(20)        NULL,
    IsActive        BIT                 NOT NULL DEFAULT 1,
    WarehouseId     UNIQUEIDENTIFIER    NOT NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_Locations PRIMARY KEY (Id),
    CONSTRAINT UQ_Locations_LocationCode UNIQUE (LocationCode),
    CONSTRAINT FK_Locations_Warehouses FOREIGN KEY (WarehouseId)
        REFERENCES Warehouses(Id) ON DELETE NO ACTION
);
GO

-- Vendors
CREATE TABLE Vendors (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    VendorCode  NVARCHAR(20)        NOT NULL,
    VendorName  NVARCHAR(200)       NOT NULL,
    Contact     NVARCHAR(200)       NULL,
    Address     NVARCHAR(500)       NULL,
    IsActive    BIT                 NOT NULL DEFAULT 1,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_Vendors PRIMARY KEY (Id),
    CONSTRAINT UQ_Vendors_VendorCode UNIQUE (VendorCode)
);
GO

-- ============================================================
-- 2. SECURITY
-- ============================================================

-- Users
CREATE TABLE Users (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    Username        NVARCHAR(50)        NOT NULL,
    Email           NVARCHAR(200)       NOT NULL,
    PasswordHash    NVARCHAR(MAX)       NOT NULL,
    Role            NVARCHAR(50)        NOT NULL DEFAULT 'Operator',
    WarehouseId     UNIQUEIDENTIFIER    NULL,
    IsActive        BIT                 NOT NULL DEFAULT 1,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_Users PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CHK_Users_Role CHECK (Role IN ('Admin','WarehouseManager','Operator','Viewer'))
);
GO

-- ============================================================
-- 3. INBOUND MODULE
-- ============================================================

-- Purchase Orders
CREATE TABLE PurchaseOrders (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PONumber    NVARCHAR(50)        NOT NULL,
    PODate      DATETIME2           NOT NULL,
    VendorId    UNIQUEIDENTIFIER    NOT NULL,
    Status      NVARCHAR(20)        NOT NULL DEFAULT 'Draft',
    Notes       NVARCHAR(1000)      NULL,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_PurchaseOrders PRIMARY KEY (Id),
    CONSTRAINT UQ_PurchaseOrders_PONumber UNIQUE (PONumber),
    CONSTRAINT FK_PurchaseOrders_Vendors FOREIGN KEY (VendorId)
        REFERENCES Vendors(Id) ON DELETE NO ACTION,
    CONSTRAINT CHK_PurchaseOrders_Status CHECK (Status IN ('Draft','Confirmed','Received','Closed'))
);
GO

-- Purchase Order Details
CREATE TABLE PurchaseOrderDetails (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PurchaseOrderId     UNIQUEIDENTIFIER    NOT NULL,
    ItemId              UNIQUEIDENTIFIER    NOT NULL,
    OrderedQty          DECIMAL(18,4)       NOT NULL,
    UOM                 NVARCHAR(20)        NOT NULL,
    UnitPrice           DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ReceivedQty         DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsDeleted           BIT                 NOT NULL DEFAULT 0,
    CreatedAt           DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt           DATETIME2           NULL,
    UpdatedBy           NVARCHAR(100)       NULL,
    CONSTRAINT PK_PurchaseOrderDetails PRIMARY KEY (Id),
    CONSTRAINT FK_PODetails_PurchaseOrders FOREIGN KEY (PurchaseOrderId)
        REFERENCES PurchaseOrders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PODetails_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION
);
GO

-- Goods Receipt Notes (GRN)
CREATE TABLE GoodsReceiptNotes (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    GRNNumber   NVARCHAR(50)        NOT NULL,
    GRNDate     DATETIME2           NOT NULL,
    POId        UNIQUEIDENTIFIER    NOT NULL,
    ReceivedBy  NVARCHAR(100)       NOT NULL,
    Status      NVARCHAR(20)        NOT NULL DEFAULT 'Draft',
    Notes       NVARCHAR(1000)      NULL,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_GoodsReceiptNotes PRIMARY KEY (Id),
    CONSTRAINT UQ_GRN_GRNNumber UNIQUE (GRNNumber),
    CONSTRAINT FK_GRN_PurchaseOrders FOREIGN KEY (POId)
        REFERENCES PurchaseOrders(Id) ON DELETE NO ACTION,
    CONSTRAINT CHK_GRN_Status CHECK (Status IN ('Draft','Completed','Cancelled'))
);
GO

-- Goods Receipt Note Details
CREATE TABLE GoodsReceiptNoteDetails (
    Id                  UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    GRNId               UNIQUEIDENTIFIER    NOT NULL,
    ItemId              UNIQUEIDENTIFIER    NOT NULL,
    ReceivedQty         DECIMAL(18,4)       NOT NULL,
    QCStatus            NVARCHAR(20)        NOT NULL DEFAULT 'Pending',
    PutAwayLocationId   UNIQUEIDENTIFIER    NULL,
    IsDeleted           BIT                 NOT NULL DEFAULT 0,
    CreatedAt           DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy           NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt           DATETIME2           NULL,
    UpdatedBy           NVARCHAR(100)       NULL,
    CONSTRAINT PK_GoodsReceiptNoteDetails PRIMARY KEY (Id),
    CONSTRAINT FK_GRNDetails_GRN FOREIGN KEY (GRNId)
        REFERENCES GoodsReceiptNotes(Id) ON DELETE CASCADE,
    CONSTRAINT FK_GRNDetails_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_GRNDetails_Locations FOREIGN KEY (PutAwayLocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION,
    CONSTRAINT CHK_GRNDetails_QCStatus CHECK (QCStatus IN ('Pending','Accepted','Rejected'))
);
GO

-- ============================================================
-- 4. INVENTORY MODULE
-- ============================================================

-- Stock Balance (current stock per item per location)
CREATE TABLE StockBalances (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ItemId          UNIQUEIDENTIFIER    NOT NULL,
    LocationId      UNIQUEIDENTIFIER    NOT NULL,
    AvailableQty    DECIMAL(18,4)       NOT NULL DEFAULT 0,
    ReservedQty     DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_StockBalances PRIMARY KEY (Id),
    CONSTRAINT UQ_StockBalances_ItemLocation UNIQUE (ItemId, LocationId),
    CONSTRAINT FK_StockBalances_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_StockBalances_Locations FOREIGN KEY (LocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION
);
GO

-- Stock Ledger (all stock movements history)
CREATE TABLE StockLedgers (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ItemId          UNIQUEIDENTIFIER    NOT NULL,
    LocationId      UNIQUEIDENTIFIER    NOT NULL,
    Quantity        DECIMAL(18,4)       NOT NULL,
    MovementType    NVARCHAR(30)        NOT NULL,
    ReferenceNumber NVARCHAR(100)       NOT NULL,
    Remarks         NVARCHAR(500)       NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_StockLedgers PRIMARY KEY (Id),
    CONSTRAINT FK_StockLedgers_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_StockLedgers_Locations FOREIGN KEY (LocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION,
    CONSTRAINT CHK_StockLedgers_MovementType CHECK (MovementType IN ('Inbound','Outbound','Transfer','Adjustment','CycleCount'))
);
GO

-- Stock Adjustments
CREATE TABLE StockAdjustments (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    AdjNumber   NVARCHAR(50)        NOT NULL,
    AdjDate     DATETIME2           NOT NULL,
    Reason      NVARCHAR(500)       NOT NULL,
    ItemId      UNIQUEIDENTIFIER    NOT NULL,
    LocationId  UNIQUEIDENTIFIER    NOT NULL,
    AdjQty      DECIMAL(18,4)       NOT NULL,
    IsApproved  BIT                 NOT NULL DEFAULT 0,
    ApprovedBy  NVARCHAR(100)       NULL,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_StockAdjustments PRIMARY KEY (Id),
    CONSTRAINT UQ_StockAdjustments_AdjNumber UNIQUE (AdjNumber),
    CONSTRAINT FK_StockAdj_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_StockAdj_Locations FOREIGN KEY (LocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION
);
GO

-- Stock Transfers
CREATE TABLE StockTransfers (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    TransferNumber  NVARCHAR(50)        NOT NULL,
    TransferDate    DATETIME2           NOT NULL,
    ItemId          UNIQUEIDENTIFIER    NOT NULL,
    FromLocationId  UNIQUEIDENTIFIER    NOT NULL,
    ToLocationId    UNIQUEIDENTIFIER    NOT NULL,
    Qty             DECIMAL(18,4)       NOT NULL,
    Notes           NVARCHAR(500)       NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_StockTransfers PRIMARY KEY (Id),
    CONSTRAINT UQ_StockTransfers_TransferNumber UNIQUE (TransferNumber),
    CONSTRAINT FK_StockTrf_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_StockTrf_FromLocation FOREIGN KEY (FromLocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_StockTrf_ToLocation FOREIGN KEY (ToLocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION
);
GO

-- Cycle Counts
CREATE TABLE CycleCounts (
    Id          UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    CountNumber NVARCHAR(50)        NOT NULL,
    CountDate   DATETIME2           NOT NULL,
    ItemId      UNIQUEIDENTIFIER    NOT NULL,
    LocationId  UNIQUEIDENTIFIER    NOT NULL,
    SystemQty   DECIMAL(18,4)       NOT NULL DEFAULT 0,
    CountedQty  DECIMAL(18,4)       NOT NULL DEFAULT 0,
    Notes       NVARCHAR(500)       NULL,
    IsAdjusted  BIT                 NOT NULL DEFAULT 0,
    IsDeleted   BIT                 NOT NULL DEFAULT 0,
    CreatedAt   DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy   NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt   DATETIME2           NULL,
    UpdatedBy   NVARCHAR(100)       NULL,
    CONSTRAINT PK_CycleCounts PRIMARY KEY (Id),
    CONSTRAINT UQ_CycleCounts_CountNumber UNIQUE (CountNumber),
    CONSTRAINT FK_CycleCounts_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_CycleCounts_Locations FOREIGN KEY (LocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION
);
GO

-- ============================================================
-- 5. OUTBOUND MODULE
-- ============================================================

-- Sales Orders
CREATE TABLE SalesOrders (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    SONumber        NVARCHAR(50)        NOT NULL,
    SODate          DATETIME2           NOT NULL,
    CustomerName    NVARCHAR(200)       NOT NULL,
    CustomerAddress NVARCHAR(500)       NULL,
    Status          NVARCHAR(20)        NOT NULL DEFAULT 'Draft',
    Notes           NVARCHAR(1000)      NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_SalesOrders PRIMARY KEY (Id),
    CONSTRAINT UQ_SalesOrders_SONumber UNIQUE (SONumber),
    CONSTRAINT CHK_SalesOrders_Status CHECK (Status IN ('Draft','Confirmed','Picking','Shipped','Closed'))
);
GO

-- Sales Order Details
CREATE TABLE SalesOrderDetails (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    SalesOrderId    UNIQUEIDENTIFIER    NOT NULL,
    ItemId          UNIQUEIDENTIFIER    NOT NULL,
    OrderedQty      DECIMAL(18,4)       NOT NULL,
    UOM             NVARCHAR(20)        NOT NULL,
    UnitPrice       DECIMAL(18,4)       NOT NULL DEFAULT 0,
    PickedQty       DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_SalesOrderDetails PRIMARY KEY (Id),
    CONSTRAINT FK_SODetails_SalesOrders FOREIGN KEY (SalesOrderId)
        REFERENCES SalesOrders(Id) ON DELETE CASCADE,
    CONSTRAINT FK_SODetails_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION
);
GO

-- Picking Lists
CREATE TABLE PickingLists (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    SalesOrderId    UNIQUEIDENTIFIER    NOT NULL,
    AssignedPicker  NVARCHAR(100)       NOT NULL,
    IsCompleted     BIT                 NOT NULL DEFAULT 0,
    CompletedAt     DATETIME2           NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_PickingLists PRIMARY KEY (Id),
    CONSTRAINT FK_PickingLists_SalesOrders FOREIGN KEY (SalesOrderId)
        REFERENCES SalesOrders(Id) ON DELETE NO ACTION
);
GO

-- Picking List Details
CREATE TABLE PickingListDetails (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PickingListId   UNIQUEIDENTIFIER    NOT NULL,
    ItemId          UNIQUEIDENTIFIER    NOT NULL,
    LocationId      UNIQUEIDENTIFIER    NOT NULL,
    RequiredQty     DECIMAL(18,4)       NOT NULL,
    PickedQty       DECIMAL(18,4)       NOT NULL DEFAULT 0,
    IsPicked        BIT                 NOT NULL DEFAULT 0,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_PickingListDetails PRIMARY KEY (Id),
    CONSTRAINT FK_PLDetails_PickingLists FOREIGN KEY (PickingListId)
        REFERENCES PickingLists(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PLDetails_Items FOREIGN KEY (ItemId)
        REFERENCES Items(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_PLDetails_Locations FOREIGN KEY (LocationId)
        REFERENCES Locations(Id) ON DELETE NO ACTION
);
GO

-- Packings
CREATE TABLE Packings (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    PackNumber      NVARCHAR(50)        NOT NULL,
    SalesOrderId    UNIQUEIDENTIFIER    NOT NULL,
    PackedBy        NVARCHAR(100)       NOT NULL,
    PackedDate      DATETIME2           NOT NULL,
    Notes           NVARCHAR(500)       NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_Packings PRIMARY KEY (Id),
    CONSTRAINT UQ_Packings_PackNumber UNIQUE (PackNumber),
    CONSTRAINT FK_Packings_SalesOrders FOREIGN KEY (SalesOrderId)
        REFERENCES SalesOrders(Id) ON DELETE NO ACTION
);
GO

-- Shipments
CREATE TABLE Shipments (
    Id              UNIQUEIDENTIFIER    NOT NULL DEFAULT NEWID(),
    ShipmentNumber  NVARCHAR(50)        NOT NULL,
    PackId          UNIQUEIDENTIFIER    NOT NULL,
    Carrier         NVARCHAR(100)       NOT NULL,
    TrackingNo      NVARCHAR(100)       NULL,
    ShippedDate     DATETIME2           NOT NULL,
    Notes           NVARCHAR(500)       NULL,
    IsDeleted       BIT                 NOT NULL DEFAULT 0,
    CreatedAt       DATETIME2           NOT NULL DEFAULT GETUTCDATE(),
    CreatedBy       NVARCHAR(100)       NOT NULL DEFAULT '',
    UpdatedAt       DATETIME2           NULL,
    UpdatedBy       NVARCHAR(100)       NULL,
    CONSTRAINT PK_Shipments PRIMARY KEY (Id),
    CONSTRAINT UQ_Shipments_ShipmentNumber UNIQUE (ShipmentNumber),
    CONSTRAINT FK_Shipments_Packings FOREIGN KEY (PackId)
        REFERENCES Packings(Id) ON DELETE NO ACTION
);
GO

-- ============================================================
-- 6. INDEXES (performance)
-- ============================================================

CREATE INDEX IX_Items_ItemCode         ON Items(ItemCode)          WHERE IsDeleted = 0;
CREATE INDEX IX_Items_Category         ON Items(Category)          WHERE IsDeleted = 0;
CREATE INDEX IX_Locations_WarehouseId  ON Locations(WarehouseId)   WHERE IsDeleted = 0;
CREATE INDEX IX_PO_VendorId            ON PurchaseOrders(VendorId) WHERE IsDeleted = 0;
CREATE INDEX IX_PO_Status              ON PurchaseOrders(Status)   WHERE IsDeleted = 0;
CREATE INDEX IX_GRN_POId               ON GoodsReceiptNotes(POId)  WHERE IsDeleted = 0;
CREATE INDEX IX_StockBalance_ItemId    ON StockBalances(ItemId)    WHERE IsDeleted = 0;
CREATE INDEX IX_StockLedger_ItemId     ON StockLedgers(ItemId)     WHERE IsDeleted = 0;
CREATE INDEX IX_StockLedger_CreatedAt  ON StockLedgers(CreatedAt)  WHERE IsDeleted = 0;
CREATE INDEX IX_SO_Status              ON SalesOrders(Status)      WHERE IsDeleted = 0;
CREATE INDEX IX_SO_CustomerName        ON SalesOrders(CustomerName) WHERE IsDeleted = 0;
CREATE INDEX IX_Users_Username         ON Users(Username)           WHERE IsDeleted = 0;
GO

-- ============================================================
-- 7. SEED DATA
-- ============================================================

-- Units of Measure
INSERT INTO UnitsOfMeasure (Id, UOMCode, UOMName, ConversionFactor, CreatedBy)
VALUES
    (NEWID(), 'PCS', 'Pieces',   1.000000, 'system'),
    (NEWID(), 'KG',  'Kilogram', 1.000000, 'system'),
    (NEWID(), 'BOX', 'Box',      1.000000, 'system'),
    (NEWID(), 'LTR', 'Liter',    1.000000, 'system'),
    (NEWID(), 'MTR', 'Meter',    1.000000, 'system');
GO

-- Warehouse
DECLARE @WarehouseId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Warehouses (Id, WarehouseCode, WarehouseName, Address, CreatedBy)
VALUES (@WarehouseId, 'WH-001', 'Main Warehouse', '123 Industrial Zone, City', 'system');

-- Locations
INSERT INTO Locations (Id, LocationCode, Aisle, Rack, [Level], WarehouseId, CreatedBy)
VALUES
    (NEWID(), 'A-01-01', 'A', '01', '01', @WarehouseId, 'system'),
    (NEWID(), 'A-01-02', 'A', '01', '02', @WarehouseId, 'system'),
    (NEWID(), 'A-02-01', 'A', '02', '01', @WarehouseId, 'system'),
    (NEWID(), 'B-01-01', 'B', '01', '01', @WarehouseId, 'system'),
    (NEWID(), 'B-01-02', 'B', '01', '02', @WarehouseId, 'system'),
    (NEWID(), 'B-02-01', 'B', '02', '01', @WarehouseId, 'system'),
    (NEWID(), 'C-01-01', 'C', '01', '01', @WarehouseId, 'system'),
    (NEWID(), 'C-02-01', 'C', '02', '01', @WarehouseId, 'system');

-- Admin User (password: Admin@123)
-- BCrypt hash generated via BCrypt.Net-Next workFactor=11
INSERT INTO Users (Id, Username, Email, PasswordHash, Role, WarehouseId, CreatedBy)
VALUES (
    NEWID(),
    'admin',
    'admin@wms.local',
    '$2a$11$rKLRdxoThJL/peeEibbuO.EOp7qX2DwBDFSYWl1Q.LrV./qETiOA2',
    'Admin',
    @WarehouseId,
    'system'
);

-- Sample Vendor
INSERT INTO Vendors (Id, VendorCode, VendorName, Contact, Address, CreatedBy)
VALUES
    (NEWID(), 'VND-001', 'PT. Supplier Utama',  '021-555-1234', 'Jl. Industri No.1, Jakarta', 'system'),
    (NEWID(), 'VND-002', 'CV. Maju Bersama',    '021-555-5678', 'Jl. Raya No.22, Surabaya',   'system');

-- Sample Items
INSERT INTO Items (Id, ItemCode, ItemName, Description, UOM, Category, MinStock, MaxStock, CreatedBy)
VALUES
    (NEWID(), 'ITM-001', 'Laptop Dell XPS 15',     '15 inch laptop',      'PCS', 'Electronics', 5,  50,  'system'),
    (NEWID(), 'ITM-002', 'Mouse Wireless Logitech', 'Wireless optical',    'PCS', 'Electronics', 10, 200, 'system'),
    (NEWID(), 'ITM-003', 'Kertas HVS A4 80gr',      'Rim 500 lembar',      'BOX', 'Stationery',  20, 500, 'system'),
    (NEWID(), 'ITM-004', 'Ballpoint Pilot BP-S',    'Blue ink, medium',    'BOX', 'Stationery',  30, 300, 'system'),
    (NEWID(), 'ITM-005', 'Mineral Water 600ml',     'Bottled water',       'BOX', 'Consumable',  50, 500, 'system');
GO

PRINT 'WMS_DB tables created and seeded successfully.';
GO
