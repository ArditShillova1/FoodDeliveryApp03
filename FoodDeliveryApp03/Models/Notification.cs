namespace FoodDeliveryApp03.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public string MenuItemPhotoUrl { get; set; }
        public string UserAddress { get; set; }
        public string UserId { get; set; }  
        public string UserFirstName { get; set; }   
        public string UserLastName { get; set; }    
        public DateTime CreatedAt { get; set; }
        public ApplicationUser User { get; set; }
    }

}
