public interface IContributionHistoryRepository : IRepository<ContributionHistory>
{
    Task<IEnumerable<ContributionHistory>> GetByCustomerIdAsync(int customerId);
}