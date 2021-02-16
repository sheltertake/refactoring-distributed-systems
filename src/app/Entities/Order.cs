namespace app.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public bool IsEventSent { get; internal set; }
    }
}
