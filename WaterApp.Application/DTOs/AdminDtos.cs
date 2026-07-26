namespace WaterApp.Application.DTOs;

public record AdminStatsResponse(
    int TotalUsers,
    int TotalBuyers,
    int TotalSellers,
    int TotalAdmins,
    int PendingSellers,
    int ApprovedSellers,
    int TotalOrders,
    decimal TotalRevenue
);

public record AdminSellerResponse(
    Guid Id,
    Guid UserId,
    string OwnerName,
    string Phone,
    string? Email,
    string CompanyName,
    string Status,
    DateTime CreatedAt
);

public record UpdateSellerStatusRequest(string Status);

public record AdminSellerResponse(
    Guid Id,
    Guid UserId,
    string OwnerName,
    string Phone,
    string? Email,
    string CompanyName,
    string Status,
    DateTime CreatedAt
);

public record UpdateSellerStatusRequest(string Status);

public record AdminUserResponse(
    Guid Id,
    string Name,
    string Phone,
    string? Email,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public record UpdateUserActiveRequest(bool IsActive);

public record AdminOrderResponse(
    Guid Id,
    string BuyerName,
    string SellerName,
    string Status,
    string PaymentMode,
    string PaymentStatus,
    decimal TotalAmount,
    DateTime CreatedAt
);
