using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryApp03.Data;
using FoodDeliveryApp03.Models;
using Microsoft.AspNetCore.Authorization;
using FoodDeliveryApp03.Extensions;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace FoodDeliveryApp03.Controllers
{
    public class MenuItemsController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MenuItemsController(DBContext context , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

            var sortedMenuItems = menuItemsWithReviews
                .OrderByDescending(m => m.AverageRating)
                .GroupBy(m => m.RestaurantName)
                .ToList();

            return View(sortedMenuItems);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Profile", new { id = menuItem.UserId });
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuItem product,IFormFile? photo)
        {

                if (product.Photo != null && product.Photo.Length > 0)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", product.Photo.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.Photo.CopyToAsync(stream);
                    }

                    product.ImagePath = "/images/" + product.Photo.FileName;
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);
                if (user is ApplicationUser applicationUser)
                {
                    product.RestaurantName = applicationUser.RestaurantName;
                    product.UserId = applicationUser.Id;
                }

            _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
           
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return View(menuItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,RestaurantName,Description,Price,ImagePath")] MenuItem menuItem)
        {
            if (id != menuItem.Id)
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user is ApplicationUser applicationUser)
            {
                menuItem.RestaurantName = applicationUser.RestaurantName;
            }
            

            try
                {
                    _context.Update(menuItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuItemExists(menuItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.MenuItems == null)
            {
                return NotFound();
            }

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (menuItem == null)
            {
                return NotFound();
            }

            return View(menuItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.MenuItems == null)
            {
                return Problem("Entity set 'DBContext.MenuItems'  is null.");
            }
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem != null)
            {
                _context.MenuItems.Remove(menuItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuItemExists(int id)
        {
          return (_context.MenuItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public IActionResult ViewProfile(string userId)
        {
            return RedirectToAction("Profile", "Profile", new { id = userId });
        }


    }
}
