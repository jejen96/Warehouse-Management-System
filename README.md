# 🏭 Warehouse Management System (WMS)

A production-ready **Warehouse Management System** REST API built with **ASP.NET Core 8**, following **Clean Architecture** principles. Designed to handle end-to-end warehouse operations including inbound, inventory, and outbound management.

---

## 🚀 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | JWT Bearer Token |
| Architecture | Clean Architecture (4 layers) |
| Pattern | Repository Pattern + Unit of Work |
| Documentation | Swagger / OpenAPI 3.0 |
| Validation | FluentValidation |
| Mapping | AutoMapper 12 |
| Logging | Serilog (Console + Rolling File) |
| Testing | xUnit + Moq + FluentAssertions |

---

## 📁 Project Structure

```
WMS.sln
├── src/
│   ├── WMS.Domain/          # Entities, Enums, Interfaces
│   ├── WMS.Application/     # Services, DTOs, Validators, Mappings
│   ├── WMS.Infrastructure/  # EF Core, Repositories, JWT, BCrypt
│   └── WMS.API/             # Controllers, Middleware, Program.cs
├── tests/
│   └── WMS.Tests/           # Unit Tests (xUnit + Moq)
└── database/
    └── WMS_CreateTables.sql # SQL Script for manual DB setup
```

---

## 📦 Modules

### 1. Master Data
- **Item (Product)** — ItemCode, ItemName, UOM, Category, Min/Max Stock
- **Warehouse** — WarehouseCode, WarehouseName, Address
- **Location (Bin)** — Aisle, Rack, Level per Warehouse
- **Vendor** — VendorCode, VendorName, Contact
- **Unit of Measure** — UOMCode, ConversionFactor

### 2. Inbound
- **Purchase Order (PO)** — Draft → Confirmed → Received → Closed
- **Goods Receipt Note (GRN)** — Receive items, QC check (Accepted/Rejected)
- **Put Away** — Assign items to bin locations after QC passed

### 3. Inventory
- **Stock Balance** — Real-time stock per Item per Location
- **Stock Ledger** — Full movement history (Inbound, Outbound, Transfer, Adjustment)
- **Stock Adjustment** — Increase/decrease stock with approval flow
- **Stock Transfer** — Move stock between locations
- **Cycle Count** — Physical count with variance detection

### 4. Outbound
- **Sales Order (SO)** — Draft → Confirmed → Picking → Shipped → Closed
- **Picking List** — Auto-generated from SO, assigned to picker
- **Packing** — Pack items for shipment
- **Shipment** — Dispatch with carrier & tracking number

### 5. Reports
- Stock Balance Report (by Item / Location / Warehouse)
- Stock Movement Report (date range filter)
- GRN Report
- Shipment Report
- Cycle Count Variance Report

### 6. Security
- **JWT Authentication** — Token includes UserId, Username, Role, WarehouseId
- **Role-Based Access** — Admin, Warehouse Manager, Operator, Viewer
- **Audit Trail** — CreatedBy, CreatedAt, UpdatedBy, UpdatedAt on all records
- **Soft Delete** — IsDeleted flag, no permanent data loss

---

## 🔌 API Endpoints

```
POST   /api/v1/auth/login
POST   /api/v1/auth/register

GET    /api/v1/items
POST   /api/v1/items
PUT    /api/v1/items/{id}
DELETE /api/v1/items/{id}

GET    /api/v1/purchase-orders
POST   /api/v1/purchase-orders
PUT    /api/v1/purchase-orders/{id}/status

GET    /api/v1/grn
POST   /api/v1/grn
POST   /api/v1/grn/{id}/complete

GET    /api/v1/stock/balances
GET    /api/v1/stock/ledger
POST   /api/v1/stock-adjustments/{id}/approve
POST   /api/v1/stock-transfers
POST   /api/v1/cycle-counts/{id}/adjust

GET    /api/v1/sales-orders
POST   /api/v1/picking-lists
POST   /api/v1/picking-lists/{id}/complete
POST   /api/v1/shipments

GET    /api/v1/reports/stock-balance
GET    /api/v1/reports/stock-movement
GET    /api/v1/reports/grn
GET    /api/v1/reports/shipments
GET    /api/v1/reports/cycle-count-variance
```

All list endpoints support **pagination** (`pageNumber`, `pageSize`) and **filtering**.

---

## ⚡ Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server / SQL Server Express

### 1. Clone the repository
```bash
git clone https://github.com/your-username/warehouse-management-system.git
cd warehouse-management-system
```

### 2. Configure database connection
Edit `src/WMS.API/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=WMS_DB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

### 3. Setup database

**Option A — EF Core Migration (recommended):**
```bash
dotnet ef migrations add InitialCreate --project src/WMS.Infrastructure --startup-project src/WMS.API
dotnet ef database update --project src/WMS.Infrastructure --startup-project src/WMS.API
```

**Option B — SQL Script:**
```bash
# Run database/WMS_CreateTables.sql on your SQL Server
```

### 4. Run the API
```bash
dotnet run --project src/WMS.API
```

### 5. Open Swagger UI
```
http://localhost:5000
```

---

## 🔐 Default Credentials

| Field | Value |
|---|---|
| Username | `admin` |
| Password | `Admin@123` |
| Role | Admin |

> Login via `POST /api/v1/auth/login` → copy token → click **Authorize** in Swagger → enter `Bearer <token>`

---

## 🧪 Running Tests

```bash
dotnet test tests/WMS.Tests
```

Test coverage includes:
- `ItemService` — CRUD, duplicate check, soft delete
- `PurchaseOrderService` — status validation, delete guard
- `StockTransferService` — insufficient stock, same-location validation
- `AuthService` — login, invalid credentials, duplicate username

---

## 🗄️ Database Design

All tables include:

```sql
Id          UNIQUEIDENTIFIER  -- Primary key (GUID)
CreatedAt   DATETIME2         -- Audit trail
CreatedBy   NVARCHAR(100)     -- Audit trail
UpdatedAt   DATETIME2         -- Audit trail
UpdatedBy   NVARCHAR(100)     -- Audit trail
IsDeleted   BIT               -- Soft delete flag
```

**21 tables** across 5 modules: Master Data, Inbound, Inventory, Outbound, Security.

---

## 📋 API Response Format

All endpoints return a standardized response:

```json
{
  "success": true,
  "data": { ... },
  "message": "Success",
  "errors": []
}
```

Paginated list response:

```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "totalCount": 100,
    "pageNumber": 1,
    "pageSize": 20,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## 📄 License

This project is licensed under the MIT License.
