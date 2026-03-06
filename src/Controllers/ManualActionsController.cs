using Microsoft.AspNetCore.Mvc;
using Services;

[ApiController]
[Route("api/admin/acoes-manuais")]
public class ManualActionController : ControllerBase
{
    private readonly IParserB3CotHist _b3ParserService;
    private readonly IRebalancingEngineService _rebalancingEngineService;

    public ManualActionController(IRebalancingEngineService rebalancingEngineService, IParserB3CotHist b3ParserService)
    {
        _b3ParserService = b3ParserService;
        _rebalancingEngineService = rebalancingEngineService;
    }

    [HttpGet("parseb3")]
    public async Task<ActionResult> ParseB3Files()
    {
        await _b3ParserService.ParseAndSyncDatabaseAsync();
        return Ok("Parse executado com sucesso. Todas as cotações estão agora no banco.");
    }

    [HttpGet("rebalance-engine")]
    public async Task<ActionResult> ExecuteRebalanceEngine()
    {
        var executed = await _rebalancingEngineService.ExecuteAsync(RebalancingType.OutofBalance);
        return Ok(executed? "Rebalanceamento executado com sucesso" : "Rebalanceamento não necessário.");
    }
}