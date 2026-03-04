using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/conta-master")]
public class MasterAccountController : ControllerBase
{
    private readonly IMasterAccountService _masterAccountService;

    public MasterAccountController(IRecommendationBasketService basketService, IMasterAccountService masterAccountService)
    {
        _masterAccountService = masterAccountService;
    }

    [HttpGet("custodia")]
    public async Task<ActionResult<MasterCustodyResponse>> GetMasterCustody()
    {
        var response = await _masterAccountService.GetMasterCustodyAsync();
        return Ok(response);
    }
}