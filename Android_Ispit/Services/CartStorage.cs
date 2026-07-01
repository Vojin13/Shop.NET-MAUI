using Newtonsoft.Json;
using Android_Ispit.DTO;

namespace Android_Ispit.Services
{
    // SecureStorage-based cart, scoped to the current logged-in session - cleared on logout
    // and on checkout, since there's no cart/order endpoint on the API.
    public static class CartStorage
    {
        private const string CartKey = "cart";
        public const int MaxQuantityPerProduct = 10;

        public static async Task<List<CartItemDTO>> GetCartAsync()
        {
            string? json = await SecureStorage.Default.GetAsync(CartKey);
            if (string.IsNullOrEmpty(json))
                return new List<CartItemDTO>();

            return JsonConvert.DeserializeObject<List<CartItemDTO>>(json) ?? new List<CartItemDTO>();
        }

        public static async Task SaveCartAsync(List<CartItemDTO> items)
        {
            await SecureStorage.Default.SetAsync(CartKey, JsonConvert.SerializeObject(items));
        }

        public static void ClearCart()
        {
            SecureStorage.Default.Remove(CartKey);
        }

        public static async Task AddToCartAsync(ProductDTO product, int quantity)
        {
            var items = await GetCartAsync();
            var existing = items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existing != null)
            {
                existing.Quantity = Math.Min(existing.Quantity + quantity, MaxQuantityPerProduct);
            }
            else
            {
                items.Add(new CartItemDTO
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    Price = product.Price,
                    ImageUrl = product.MainImage,
                    Quantity = Math.Min(quantity, MaxQuantityPerProduct)
                });
            }
            await SaveCartAsync(items);
        }

        public static async Task<int> GetItemCountAsync()
        {
            var items = await GetCartAsync();
            return items.Sum(i => i.Quantity);
        }
    }
}
