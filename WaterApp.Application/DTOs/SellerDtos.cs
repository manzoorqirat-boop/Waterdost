namespace WaterApp.Application.DTOs;

public record SellerProfileDto(
    Guid Id,
    string CompanyName,
    string Status,
    string? LogoUrl,
    double BaseLatitude,
    double BaseLongitude,
    List<string> ServicePincodes,
    DateTime CreatedAt
);

public record ProductUpdateRequest(string Name, string VolumeLabel, decimal Price, int StockQty, bool IsActive);

public record SellerOrderItemDto(
    Guid ProductId,
    string ProductName,
    string VolumeLabel,
    int Quantity,
    decimal PriceAtPurchase
);

public record SellerOrderDto(
    Guid Id,
    string BuyerName,
    string BuyerPhone,
    string Status,
    string PaymentMode,
    string PaymentStatus,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime? DeliveredAt,
    string? AddressSummary,
    List<SellerOrderItemDto> Items
);

public record UpdateOrderStatusRequest(string Status);

public record SellerDashboardStatsDto(
    int TotalProducts,
    int ActiveProducts,
    int LowStockProducts,
    int PendingOrders,
    int TotalOrders,
    int TodayOrders,
    decimal TotalRevenue,
    decimal TodayRevenue
);
