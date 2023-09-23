namespace FoodDeliveryApp03.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public List<CartItem> Items { get; set; }
        public Order Order { get; set; }
        public Cart()
        {
            Items = new List<CartItem>();
        }
    }
}
