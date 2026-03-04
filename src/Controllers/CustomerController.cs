using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/clientes")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost("adesao")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var response = await _customerService.CreateAsync(request);
        return CreatedAtAction(nameof(Create), new { id = response.ClienteId }, response);
    }

    [HttpPost("{clienteId}/saida")]
    public async Task<IActionResult> Leave(int clienteId)
    {
        var response = await _customerService.LeaveInvestmentProductAsync(clienteId);
        return Ok(response);
    }

    [HttpPut("{clienteId}/valor-mensal")]
    public async Task<IActionResult> UpdateAmount(int clienteId, [FromBody] decimal newValue)
    {
        var response = await _customerService.UpdateMonthlyAmountAsync(clienteId, newValue);
        return Ok(response);
    }

    [HttpGet("{clienteId}/carteira")]
    public async Task<IActionResult> GetPortfolio(int clienteId)
    {
        var summary = await _customerService.GetPortfolioSummaryAsync(clienteId);
        return Ok(summary);
    }

    [HttpGet("{clienteId}/rentabilidade")]
    public async Task<IActionResult> GetProfitability(int clienteId)
    {
        var report = await _customerService.GetDetailedProfitabilityAsync(clienteId);
        return Ok(report);
    }

    
}