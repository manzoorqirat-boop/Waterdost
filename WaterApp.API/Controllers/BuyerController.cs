using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WaterApp.Application.DTOs;
using WaterApp.Application.Interfaces;

namespace WaterApp.API.Controllers;

[ApiController]
[Route("api/buyer")]
[Authorize(Roles = "Buyer")]
public class BuyerController : ControllerBase
{
    private readonly IBuyerService _buyerService;

    public BuyerController(IBuyerService buyerService)
    {
        _buyerService = buyerService;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ---- Addresses ----

    [HttpGet("addresses")]
    public async Task<ActionResult<List<AddressDto>>> GetAddresses()
    {
        return Ok(await _buyerService.GetMyAddressesAsync(CurrentUserId));
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<AddressDto>> AddAddress(AddressCreateRequest request)
    {
        try
        {
            return Ok(await _buyerService.AddAddressAsync(CurrentUserId, request));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("addresses/{id}")]
    public async Task<ActionResult<AddressDto>> UpdateAddress(Guid id, AddressUpdateRequest request)
    {
        try
        {
            return Ok(await _buyerService.UpdateAddressAsync(CurrentUserId, id, request));
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

    [HttpDelete("addresses/{id}")]
    public async Task<IActionResult> DeleteAddress(Guid id)
    {
        try
        {
            await _buyerService.DeleteAddressAsync(CurrentUserId, id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPost("addresses/{id}/default")]
    public async Task<ActionResult<AddressDto>> SetDefaultAddress(Guid id)
    {
        try
        {
            return Ok(await _buyerService.SetDefaultAddressAsync(CurrentUserId, id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
