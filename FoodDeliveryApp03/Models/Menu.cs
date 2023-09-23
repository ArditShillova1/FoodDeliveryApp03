using System.ComponentModel.DataAnnotations.Schema;

namespace FoodDeliveryApp03.Models
{
    public class Menu
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } 
        public string ImagePath { get; set; }
    }
}
