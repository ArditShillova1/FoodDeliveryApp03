using FoodDeliveryApp03.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FoodDeliveryApp03.Models;
using Microsoft.Extensions.FileProviders;
using ServiceStack;
using FoodDeliveryApp03.Controllers;
using FoodDeliveryApp03.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DBContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("FoodDeliveryApp")));

builder.Services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DBContext>()
    .AddSignInManager<CustomSignInManager>();

builder.Services.AddTransient<CartsController>();
builder.Services.AddSignalR();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireRole("Admin");
    });
});
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(
        name: "notifications",
        pattern: "Notifications/{action=Index}/{id?}",
        defaults: new { controller = "Notifications" });
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<RestaurantNotificationHub>("/restaurantNotificationHub");
});
    
app.Run();
