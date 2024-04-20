using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FoodDeliveryApp03.Data;
using FoodDeliveryApp03.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryApp03.Controllers
{
    //[Authorize]
    public class ProfileController : Controller
    {
        private readonly DBContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(DBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var profile = await _context.ProfilePicture.FirstOrDefaultAsync(p => p.UserId == userId);
            var menuItems = await _context.MenuItems.Where(m => m.UserId == userId).ToListAsync();

            ViewBag.Profile = profile;
            ViewBag.MenuItems = menuItems;

            return View();
        }




        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfilePicture profilePicture, IFormFile photo)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingProfilePicture = await _context.ProfilePicture.FirstOrDefaultAsync(p => p.UserId == userId);

            if (existingProfilePicture != null)
            {
                _context.ProfilePicture.Remove(existingProfilePicture);
                await _context.SaveChangesAsync();
            }

            if (photo != null && photo.Length > 0)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", photo.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                profilePicture.ImagePath = "/images/" + photo.FileName;
            }

            profilePicture.UserId = userId;
            var user = await _userManager.FindByIdAsync(userId);

            if (user is ApplicationUser applicationUser)
            {
                profilePicture.RestaurantName = applicationUser.RestaurantName;
            }

            _context.ProfilePicture.Add(profilePicture);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.ProfilePicture.FirstOrDefaultAsync(p => p.UserId == id);
            if (profile == null)
            {
                return NotFound();
            }

            var menuItems = await _context.MenuItems.Where(m => m.UserId == id).ToListAsync();

            ViewBag.Profile = profile;
            ViewBag.MenuItems = menuItems;

            return View(profile); 
            }



    }
}
