using CommunityToolkit.Mvvm.ComponentModel;

namespace Android_Ispit.DTO
{
    // Unlike other DTOs, this one is observable on purpose: quantity changes on the Cart page
    // need to update the bound Label/total live without replacing the whole collection item.
    public partial class CartItemDTO : ObservableObject
    {
        public int ProductId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LineTotalDisplay))]
        private int _quantity;

        public string PriceDisplay => $"${Price:0.##}";
        public string LineTotalDisplay => $"${Price * Quantity:0.##}";
    }
}
