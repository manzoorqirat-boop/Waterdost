namespace WaterApp.Application.DTOs;

// ---- Addresses ----

public record AddressDto(
    Guid Id,
    string Line1,
    string? Line2,
    string City,
    string State,
    string Pincode,
    double? Latitude,
    double? Longitude,
    bool IsDefault
);

public record AddressCreateRequest(
    string Line1,
    string? Line2,
    string City,
    string State,
    string Pincode,
    double? Latitude,
    double? Longitude,
    bool IsDefault
);

public record AddressUpdateRequest(
    string Line1,
    string? Line2,
    string City,
    string State,
    string Pincode,
    double? Latitude,
    double? Longitude,
    bool IsDefault
);

// ---- Order detail (buyer view) ----

public record BuyerOrderItemDto(
    Guid ProductId,
    string ProductName,
    string VolumeLabel,
    int Quantity,
    decimal PriceAtPurchase
);

public record BuyerOrderDetailDto(
    Guid Id,
    Guid SellerId,
    string SellerName,
    string Status,
    string PaymentMode,
    string PaymentStatus,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime? DeliveredAt,
    string? AddressSummary,
    List<BuyerOrderItemDto> Items
);

// ---- Reviews ----

public record SellerReviewDto(
    Guid Id,
    string BuyerName,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);

public record CreateReviewRequest(int Rating, string? Comment);

// ---- Cart ----

public record UpdateCartItemRequest(int Quantity);
