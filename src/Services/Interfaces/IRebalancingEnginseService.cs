public interface IRebalancingEngineService
{
    Task<bool> ExecuteAsync(RebalancingType rebalancingType);
}