public interface ICustomerService
{
    Task<int> CreateAsync(string name, string document, string email, decimal contribution);


    Task UpdateMonthlyAmountAsync(int customerId, decimal newAmount);

    Task UpdateSubscriptionState(int customerId, bool state);

    Task<object> GetPortfolioSummaryAsync(int customerId);

    Task<PortfolioProfitabilityResponse> GetDetailedProfitabilityAsync(int customerId);
}