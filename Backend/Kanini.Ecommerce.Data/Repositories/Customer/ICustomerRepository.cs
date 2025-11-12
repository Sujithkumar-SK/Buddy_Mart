using Kanini.Ecommerce.Common;
using DomainCustomer = Kanini.Ecommerce.Domain.Entities.Customer;

namespace Kanini.Ecommerce.Data.Repositories.Customer;

public interface ICustomerRepository
{
    // ADO.NET Read Operations
    Task<Result<DomainCustomer>> GetCustomerByIdAsync(int customerId);
    Task<Result<DomainCustomer>> GetCustomerByUserIdAsync(int userId);
    Task<Result<List<DomainCustomer>>> GetAllCustomersAsync();

    // EF Core Write Operations
    Task<Result> UpdateCustomerAsync(DomainCustomer customer);
    Task<Result> DeleteCustomerAsync(int customerId);
}
