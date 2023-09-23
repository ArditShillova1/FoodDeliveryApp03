using FoodDeliveryApp03.Data;
using FoodDeliveryApp03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoodDeliveryApp03.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DBContext _context;
        public HomeController(ILogger<HomeController> logger , DBContext dBContext)
        {
            _context = dBContext;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var menuItemsWithReviews = await _context.MenuItems
                .Include(m => m.Reviews) 
                .ToListAsync();

            foreach (var menuItem in menuItemsWithReviews)
            {
                menuItem.AverageRating = menuItem.Reviews.Any()
                    ? menuItem.Reviews.Average(r => r.Rating)
                    : 0;
            }

            var topRatedFoodItems = menuItemsWithReviews
                .OrderByDescending(m => m.AverageRating)
                .Take(5)
                .ToList();

            return View(topRatedFoodItems);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Profile()
        {

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}