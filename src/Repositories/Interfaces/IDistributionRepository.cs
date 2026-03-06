namespace Repositories.Interfaces
{
    public interface IDistributionRepository : IRepository<Distribution>
    {
        Task<Distribution?> GetLatestDistributionByTickerAsync(string symbol);
        Task<IEnumerable<Distribution>> GetAllDistributionsToChildAccount(int childAccountId);
    
    }
}