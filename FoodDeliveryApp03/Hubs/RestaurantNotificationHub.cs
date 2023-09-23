using Microsoft.AspNetCore.SignalR;

namespace FoodDeliveryApp03.Hubs
{
    public class RestaurantNotificationHub:Hub
    {
        public async Task SendOrderNotification(string restaurantId, string orderDetails)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, restaurantId);
            await Clients.Group(restaurantId).SendAsync("ReceiveOrderNotification", orderDetails);
        }



        public async Task JoinRestaurantGroup(string restaurantId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, restaurantId);
        }

        public async Task LeaveRestaurantGroup(string restaurantId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, restaurantId);
        }
    }
}

