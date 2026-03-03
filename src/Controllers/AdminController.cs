using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/cesta")]
public class BasketsController : ControllerBase
{
    private readonly IRecommendationBasketService _basketService;

    public BasketsController(IRecommendationBasketService basketService)
    {
        _basketService = basketService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBasketRequest request)
    {
        var response = await _basketService.CreateAsync(request);
        
        return CreatedAtAction(nameof(GetActive), new { id = response.cestaId }, response);
    }

    [HttpGet("atual")]     
    public async Task<IActionResult> GetActive()
    {
        var response = await _basketService.GetActiveBasketAsync();
        
        return Ok(response);
    }

    [HttpGet("historico")]
    public async Task<IActionResult> GetHistory()
    {
        var response = await _basketService.GetHistoryAsync();
        
        return Ok(response);
    }
}