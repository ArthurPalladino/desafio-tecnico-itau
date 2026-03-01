namespace Repositories.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {

        Task<IEnumerable<Customer>> GetActiveCustomers();
    }
}