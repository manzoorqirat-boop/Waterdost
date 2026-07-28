using WaterApp.Domain.Enums;

namespace WaterApp.Application.DTOs;

public record AddToCartRequest(Guid ProductId, int Quantity);

public record CartItemDto(Guid ProductId, string ProductName, decimal Price, int Quantity, Guid SellerId, string SellerName);

public record CartDto(Guid CartId, List<CartItemDto> Items, decimal Total);

public record PlaceOrderRequest(Guid SellerId, Guid AddressId, PaymentMode PaymentMode);

public record OrderDto(Guid Id, string Status, string PaymentMode, string PaymentStatus, decimal TotalAmount, DateTime CreatedAt);
