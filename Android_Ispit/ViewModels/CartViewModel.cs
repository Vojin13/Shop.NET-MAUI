using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Android_Ispit.DTO;
using Android_Ispit.Services;

namespace Android_Ispit.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasItems))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        [NotifyPropertyChangedFor(nameof(Total))]
        [NotifyPropertyChangedFor(nameof(TotalDisplay))]
        [NotifyPropertyChangedFor(nameof(ItemCountText))]
        private ObservableCollection<CartItemDTO> _items = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasPurchaseMessage))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private string _purchaseMessage = string.Empty;

        public bool HasItems => Items.Count > 0;
        public bool HasPurchaseMessage => !string.IsNullOrEmpty(PurchaseMessage);
        public bool ShowEmptyMessage => !HasItems && !HasPurchaseMessage;

        public decimal Total => Items.Sum(i => i.Price * i.Quantity);
        public string TotalDisplay => $"${Total:0.##}";
        public string ItemCountText => $"{Items.Count} item{(Items.Count == 1 ? "" : "s")}";

        public async Task LoadCartAsync()
        {
            PurchaseMessage = string.Empty;
            var items = await CartStorage.GetCartAsync();
            Items = new ObservableCollection<CartItemDTO>(items);
        }

        [RelayCommand]
        private async Task IncrementAsync(CartItemDTO item)
        {
            if (item == null || item.Quantity >= CartStorage.MaxQuantityPerProduct)
                return;

            item.Quantity++;
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(TotalDisplay));
            await CartStorage.SaveCartAsync(Items.ToList());
        }

        [RelayCommand]
        private async Task DecrementAsync(CartItemDTO item)
        {
            if (item == null)
                return;

            if (item.Quantity <= 1)
            {
                await RemoveAsync(item);
                return;
            }

            item.Quantity--;
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(TotalDisplay));
            await CartStorage.SaveCartAsync(Items.ToList());
        }

        [RelayCommand]
        private async Task RemoveAsync(CartItemDTO item)
        {
            if (item == null)
                return;

            Items.Remove(item);
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(ShowEmptyMessage));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(TotalDisplay));
            OnPropertyChanged(nameof(ItemCountText));
            await CartStorage.SaveCartAsync(Items.ToList());
        }

        [RelayCommand]
        private void Checkout()
        {
            CartStorage.ClearCart();
            Items = new ObservableCollection<CartItemDTO>();
            PurchaseMessage = "Purchased! Thank you for your order.";
        }
    }
}
