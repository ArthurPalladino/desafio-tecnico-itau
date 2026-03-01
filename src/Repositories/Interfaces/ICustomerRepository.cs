namespace Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {

        Task<IEnumerable<Customer>> GetActiveCustomers();
        Task<Customer?> GetCustomerWithPortfolioAsync(int customerId);
    }
}