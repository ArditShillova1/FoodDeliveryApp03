using FoodDeliveryApp03.Data;
using FoodDeliveryApp03.Extensions;
using FoodDeliveryApp03.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using ServiceStack.Messaging;
using IdentityServer4.EntityFramework.Entities;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using FoodDeliveryApp03.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Order = FoodDeliveryApp03.Models.Order;
using Microsoft.Exchange.WebServices.Data;

namespace FoodDeliveryApp03.Controllers
{
    [Authorize]
    public class CartsController : Controller
    {
        private readonly IHubContext<RestaurantNotificationHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly DBContext _context;
        public CartsController(DBContext context, UserManager<IdentityUser> userManager, IHubContext<RestaurantNotificationHub> hubContext)
        {
            _userManager = userManager;
            _context = context;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart") ?? new Cart();
            return View(cart);
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult AddToCart(int menuItemId)
        {

            var menuItem = _context.MenuItems.Find(menuItemId);

            if (menuItem != null)
            {

                var cart = HttpContext.Session.GetObject<Cart>("Cart")?? new Cart();

                var existingCartItem = cart.Items.FirstOrDefault(item => item.MenuItem.Id == menuItemId);

                if (existingCartItem != null)
                {

                    existingCartItem.Quantity++;
                }
                else
                {

                    var newCartItem = new CartItem
                    {
                        MenuItem = menuItem,
                        Quantity = 1
                    };
                    cart.Items.Add(newCartItem);
                }


                HttpContext.Session.SetObject("Cart", cart);
            }

            return RedirectToAction("Index", "Carts");
        }



        public IActionResult RemoveFromCart(int menuItemId)
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");

            if (cart != null)
            {
                var cartItem = cart.Items.FirstOrDefault(item => item.MenuItem.Id == menuItemId);

                if (cartItem != null)
                {
                    cart.Items.Remove(cartItem);

                    if (cart.Items.Count == 0)
                    {
                        HttpContext.Session.Remove("Cart");
                    }
                    else
                    {
                        HttpContext.Session.SetObject("Cart", cart);
                    }
                }
            }
                
            return RedirectToAction("Index");
        }
        
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Index");
        }


        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");

            if (cart == null || cart.Items.Count == 0)
            {
                return NotFound();
            }

            var totalPrice = cart.Items.Sum(item => item.MenuItem.Price * item.Quantity);

            ViewBag.TotalPrice = totalPrice;

            return View(cart);
        }

        [HttpPost]
        public IActionResult Checkout(string paymentMethod)
        {
            if (paymentMethod == "paypal")
            {
                return RedirectToAction("CreatePayPalOrder");
            }

            if (paymentMethod == "dummy")
            {
                return RedirectToAction("PlaceOrder");
            }
            return BadRequest();
        }

        public async Task<IActionResult> CreatePayPalOrder()
        {

            var clientId = "AbXB_D_Y3M-andpQvB9CC98YKLl9w7N_Y-Em_MUMPEY1_UaKIMYCX7iRKoFP9Phbac3fAGzQHo-qkdk1";
            var clientSecret = "EMrGnaJlj2SrwXVGEy5kfS0NvcOqYpshiszAgTM7OJZP_9uzc2YjOT56oPCouiNXMdJAFXYKLtKrsyHK";

            var cart = HttpContext.Session.GetObject<Cart>("Cart");

            if (cart == null || cart.Items.Count == 0)
            {
                return NotFound();
            }

            var totalPrice = cart.Items.Sum(item => item.MenuItem.Price * item.Quantity);

            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    BrandName = "Food Delivery App",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = "USD",
                    Value = totalPrice.ToString("0.00"),
                }
            }
        }
            };

            var environment = new SandboxEnvironment(clientId, clientSecret);
            var client = new PayPalHttpClient(environment);

            var request = new OrdersCreateRequest();
            request.Headers.Add("prefer", "return=representation");
            request.RequestBody(orderRequest);

            try
            {
                var response = await client.Execute(request);
                var order = response.Result<PayPalCheckoutSdk.Orders.Order>();
                return Redirect(order.Links.Find(x => x.Rel == "approve").Href);
            }
            catch (HttpException e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = HttpContext.Session.GetObject<Cart>("Cart");

            if (cart == null || cart.Items.Count == 0)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User) as ApplicationUser;

            var order = new Order
            {
                UserId = user.Id,
                UserFirstName = user.FirstName,
                UserLastName = user.LastName,
                UserAddress = user.Address,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Items.Sum(item => item.MenuItem.Price * item.Quantity),
                Status = OrderStatus.Pending
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var cartItemsByRestaurant = cart.Items
                .GroupBy(cartItem => cartItem.MenuItem.UserId)
                .ToList();

            foreach (var restaurantCartItems in cartItemsByRestaurant)
            {
                var restaurantId = restaurantCartItems.Key;
                var restaurantUser = await _userManager.FindByIdAsync(restaurantId) as ApplicationUser;

                var notifications = new List<Notification>();

                foreach (var cartItem in restaurantCartItems)
                {
                    var menuItem = cartItem.MenuItem;

                    var notification = new Notification
                    {
                        OrderId = order.Id,
                        TotalAmount = menuItem.Price * cartItem.Quantity,
                        MenuItemPhotoUrl = menuItem.ImagePath,
                        UserAddress = user.Address,
                        UserId = restaurantId,
                        UserFirstName = user.FirstName,
                        UserLastName = user.LastName,
                        CreatedAt = DateTime.UtcNow,
                    };

                    notifications.Add(notification);
                }

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.User(restaurantId).SendAsync("ReceiveOrderNotifications", notifications);
            }

            HttpContext.Session.Remove("Cart");

            return View("OrderConfirmation");
        }


    }

}

