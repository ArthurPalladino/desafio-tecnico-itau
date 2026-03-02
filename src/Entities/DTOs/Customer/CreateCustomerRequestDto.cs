public record CreateCustomerRequest(
    string Name, 
    string Cpf, 
    string Email, 
    decimal Contribution);