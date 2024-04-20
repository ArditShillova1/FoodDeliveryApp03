using Azure.Search.Documents.Models;
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


        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            var searchResults = new SearchResults();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var restaurantResults = _context.ProfilePicture
                    .Where(u => !string.IsNullOrEmpty(u.RestaurantName) && u.RestaurantName.Contains(searchTerm))
                    .ToList();

                searchResults.RestaurantName = restaurantResults;

                var menuItems = _context.MenuItems
                    .Where(m => m.Name.Contains(searchTerm))
                    .ToList();
                searchResults.MenuItems = menuItems;
            }
            if (string.IsNullOrWhiteSpace(searchTerm))
            {

                ViewData["Message"] = "Please enter a valid search term.";
                return View();
            }

                return View("SearchResults", searchResults);
        }





    }
}