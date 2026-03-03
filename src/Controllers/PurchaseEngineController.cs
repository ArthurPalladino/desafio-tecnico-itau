using Microsoft.AspNetCore.Mvc;
using ItauCompraProgramada.Application.Interfaces;
using Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using Services;


[ApiController]
[Route("api/motor")]
public class PurchaseEngineController : ControllerBase
{
    private readonly IParserB3CotHist _parser;
    private readonly IPurchaseEngineService _purchaseEngine;

    public PurchaseEngineController(
        IParserB3CotHist parser,
        IPurchaseEngineService purchaseEngine)
    {
        _parser = parser;
        _purchaseEngine = purchaseEngine;
    }

    [HttpPost("executar-compra")]
    public async Task<IActionResult> MotorExecute([FromQuery] DateTime dataReferencia)
    {
        await _parser.ParseAndSyncDatabaseAsync(dataReferencia); 
        await  _purchaseEngine.ExecuteAsync(dataReferencia);
        return Ok(new { 
            Message = "Processamento do dia concluído com sucesso!", 
            Date = dataReferencia.ToString("yyyy-MM-dd") 
        });
    }
}