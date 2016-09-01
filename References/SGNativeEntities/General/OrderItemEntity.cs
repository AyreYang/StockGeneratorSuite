using SGNativeEntities.Enums;

namespace SGNativeEntities.General
{
    public class OrderItemEntity
    {
        public OrderItemEntity()
        {
            this.Amount = decimal.Zero;
            this.Price = decimal.Zero;
            this.Flag = TRADE.none;
        }

        public decimal Amount { get; set; }

        public decimal Price { get; set; }

        public TRADE Flag { get; set; }
    }
}
