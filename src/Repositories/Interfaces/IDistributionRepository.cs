namespace Repositories.Interfaces
{
    public interface IDistributionRepository : IRepository<Distribution>
    {
        Task<Distribution?> GetLatestDistributionByTickerAsync(string symbol);
    }
}