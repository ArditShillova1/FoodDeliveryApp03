using ServiceStack;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FoodDeliveryApp03.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public decimal TotalAmount { get; set; }    
        public List<OrderItem> OrderItems { get; set; }
        public OrderStatus Status { get; set; }

    }
    public enum OrderStatus
    {
        Pending,
        Processing,
        Completed,
        Cancelled
    }

    

}
