namespace OrderManagement.Application.DTO
{
    public record CreateOrderDTO(int CustomerId, List<int> ProductIds);
}
