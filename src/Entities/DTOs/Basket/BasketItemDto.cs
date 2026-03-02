public record CreateBasketRequest(string Name, List<BasketItemDto> Items);
public record BasketItemDto(string Symbol, decimal Percentage);