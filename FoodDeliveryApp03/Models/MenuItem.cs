using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FoodDeliveryApp03.Models
{
    public class MenuItem
    {

            [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }  
            public decimal Price { get; set; }  
            public string UserId { get; set; }
            public ApplicationUser User { get; set; }
            public string RestaurantName { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ImagePath { get; set; }
            [NotMapped]
            public IFormFile Photo { get; set; }
            public double AverageRating { get; set; }
            public ICollection<Review> Reviews { get; set; }
            public List<Cart> Carts { get; set; }
        
    }
}
