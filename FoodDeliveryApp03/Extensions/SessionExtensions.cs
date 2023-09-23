using System.Text.Json;

namespace FoodDeliveryApp03.Extensions
{
    public static class SessionExtensions
    {
        public static T GetObject<T>(this ISession session, string key)
        {
            var data = session.GetString(key);
            return data != null ? JsonSerializer.Deserialize<T>(data) : default;
        }

        public static void SetObject<T>(this ISession session, string key, T value)
        {
            var data = JsonSerializer.Serialize(value);
            session.SetString(key, data);
        }
    }
}
