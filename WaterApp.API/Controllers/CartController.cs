using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;

namespace WaterApp.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize(Roles = "Buyer")]
public class CartController : ControllerBase
{
    private readonly IBuyerService _buyerService;

    public CartController(IBuyerService buyerService)
    {
        _buyerService = buyerService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        return Ok(await _buyerService.GetCartAsync(CurrentUserId));
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(AddToCartRequest request)
    {
        try
        {
            return Ok(await _buyerService.AddToCartAsync(CurrentUserId, request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{productId}")]
    public async Task<ActionResult<CartDto>> UpdateItem(Guid productId, UpdateCartItemRequest request)
    {
        try
        {
            return Ok(await _buyerService.UpdateCartItemAsync(CurrentUserId, productId, request.Quantity));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{productId}")]
    public async Task<ActionResult<CartDto>> RemoveItem(Guid productId)
    {
        try
        {
            return Ok(await _buyerService.RemoveCartItemAsync(CurrentUserId, productId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        await _buyerService.ClearCartAsync(CurrentUserId);
        return NoContent();
    }
}
