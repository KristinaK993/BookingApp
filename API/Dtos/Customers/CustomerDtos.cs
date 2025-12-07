namespace API.Dtos.Customers;

public class CustomerDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
}

public class CreateCustomerRequest
{
    public int CompanyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
}

public class UpdateCustomerRequest : CreateCustomerRequest
{
    public int Id { get; set; }
}
