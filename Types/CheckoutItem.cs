namespace ShoesStore.Types;



public class CheckoutItems
{
    public required CheckoutItem[] Items { get; set; }
}

public class CheckoutItem
{
    public required string ProductUUID { get; set; }

    public required int Quantity { get; set; }
}