namespace Repositories.Interfaces
{
    public interface ICustodyRepository : IRepository<Custody>
    {
        Task<Custody?> GetByAccountAndTickerAsync(int accountId, string ticker);
    }
}