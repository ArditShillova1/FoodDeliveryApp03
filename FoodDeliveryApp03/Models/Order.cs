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

        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserAddress { get; set; }

        public List<Notification> Notifications { get; set; }
        public ApplicationUser User { get; set; }
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
