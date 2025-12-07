using API.Dtos.Customers;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync();
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest dto);
    Task<CustomerDto?> UpdateAsync(int id, UpdateCustomerRequest dto);
    Task<bool> DeleteAsync(int id);
}
