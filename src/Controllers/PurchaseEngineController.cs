using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using Services;


[ApiController]
[Route("api/motor")]
public class PurchaseEngineController : ControllerBase
{
    private readonly IPurchaseEngineService _purchaseEngine;

    public PurchaseEngineController(
        IPurchaseEngineService purchaseEngine)
    {
        _purchaseEngine = purchaseEngine;
    }

    [HttpPost("executar-compra")]
    public async Task<IActionResult> MotorExecute([FromQuery] DateTime dataReferencia)
    {
        return Ok(await  _purchaseEngine.ExecuteAsync(dataReferencia));

    }
}