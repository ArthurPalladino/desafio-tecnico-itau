
public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);


    Task<UpdateAmountResponse> UpdateMonthlyAmountAsync(int customerId, decimal newAmount);

    Task<SubscriptionChangeResponse> LeaveInvestmentProductAsync(int customerId);

    Task<PortfolioSummaryResponse> GetPortfolioSummaryAsync(int customerId);

    Task<PortfolioProfitabilityResponse> GetDetailedProfitabilityAsync(int customerId);
}