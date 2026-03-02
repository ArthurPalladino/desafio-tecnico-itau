using Microsoft.AspNetCore.Mvc;
namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customerId = await _customerService.CreateAsync(
                request.Name, 
                request.Cpf, 
                request.Email, 
                request.Contribution);

            return CreatedAtAction(nameof(Create), new { id = customerId }, new { id = customerId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Um erro ocorreu ao tentar criar usuário.", detail = ex.Message });
        }
    }

    [HttpGet("{id}/portfolio")]
    public async Task<IActionResult> GetPortfolio(int id)
    {
        try
        {
            var summary = await _customerService.GetPortfolioSummaryAsync(id);
            return Ok(summary);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Customer not found." });
        }
    }

    [HttpPut("{id}/subscription")]
    public async Task<IActionResult> ChangeSubscription(int id, [FromBody] bool active)
    {
        try
        {
            await _customerService.UpdateSubscriptionState(id,active);
            return Ok(new { message = $"Assinatura {(active ? "ativada" : "desativada")} com sucesso!" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/profitability")]
public async Task<IActionResult> GetProfitability(int id)
{
    try
    {
        var report = await _customerService.GetDetailedProfitabilityAsync(id);
        return Ok(report);
    }
    catch (KeyNotFoundException)
    {
        return NotFound(new { message = "Cliente não encontrado." });
    }
}
}
