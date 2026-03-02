using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IBasketService _basketService;

    public BasketsController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketRequest request)
    {
        try
        {
            var basketId = await _basketService.CreateAsync(request);
            
            return CreatedAtAction(nameof(GetActive), new { id = basketId }, new { id = basketId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro interno ao criar cesta.", detail = ex.Message });
        }
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var basket = await _basketService.GetActiveBasketAsync();
        
        if (basket == null)
            return NotFound(new { message = "Nenhuma cesta ativa no momento." });

        return Ok(basket);
    }
}