using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryApp03.Models
{
    public class ProfilePicture
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public string ImagePath { get; set; }
        [NotMapped]
        public IFormFile Picture { get; set; }
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }
        public string  Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
