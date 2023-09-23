using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using FoodDeliveryApp03.Models;

namespace FoodDeliveryApp03.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> BanTemporary(string userId, int hours)
        {
            var user = await _userManager.FindByIdAsync(userId) as ApplicationUser ;
            if (user != null)
            {
                var banExpiration = DateTime.UtcNow.AddHours(hours);
                user.IsBanned = true;
                user.BanExpirationDate = banExpiration;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BanPermanent(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) as ApplicationUser;
            if (user != null)
            {
                user.IsBanned = true;
                user.BanExpirationDate = null; 
                await _userManager.UpdateAsync(user);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User has been temporarily banned.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to ban the user.";
                }
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Unban(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) as ApplicationUser;
            if (user != null)
            {
                user.IsBanned = false;
                user.BanExpirationDate = null;
                await _userManager.UpdateAsync(user);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "User has been unbanned.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to unban the user.";
                }
            }
                
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId) as ApplicationUser;
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    // Handle error
                }
            }

            return RedirectToAction("Index");
        }
    }
}
