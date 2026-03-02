namespace Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetCustomerByCpf(string cpf);
        Task<IEnumerable<Customer>> GetActiveCustomers();
        Task<Customer?> GetCustomerWithPortfolioAsync(int customerId);
    }
}