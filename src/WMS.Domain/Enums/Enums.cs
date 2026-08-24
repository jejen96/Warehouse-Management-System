namespace WMS.Domain.Enums;

public enum POStatus { Draft, Confirmed, Received, Closed }
public enum GRNStatus { Draft, Completed, Cancelled }
public enum QCStatus { Pending, Accepted, Rejected }
public enum SOStatus { Draft, Confirmed, Picking, Shipped, Closed }
public enum AdjustmentType { Increase, Decrease }
public enum StockMovementType { Inbound, Outbound, Transfer, Adjustment, CycleCount }
public enum UserRole { Admin, WarehouseManager, Operator, Viewer }
