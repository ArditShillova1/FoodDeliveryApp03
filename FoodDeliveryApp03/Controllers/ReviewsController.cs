using FoodDeliveryApp03.Data;
using FoodDeliveryApp03.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize] 
public class ReviewsController : Controller
{
    private readonly DBContext _context;

    public ReviewsController(DBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult SubmitReviewForMenuItem(int menuItemId)
    {
        var menuItem = _context.MenuItems.Include(m => m.Reviews).FirstOrDefault(m => m.Id == menuItemId);
        if (menuItem == null)
        {
            return NotFound();
        }

        return View(menuItem);
    }

    [HttpPost]
    public IActionResult SubmitReviewForMenuItem(int menuItemId, int rating , string reviewText)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var menuItem = _context.MenuItems.FirstOrDefault(m => m.Id == menuItemId);
        if (menuItem == null)
        {
            return NotFound();
        }

        var review = new Review
        {
            UserId = userId,
            Rating = rating,
            MenuItemId = menuItemId,
            CreatedAt = DateTime.UtcNow 
        };

        _context.Reviews.Add(review);
        _context.SaveChanges();

        return RedirectToAction("Index", "MenuItems", new { id = menuItemId });
    }
}
