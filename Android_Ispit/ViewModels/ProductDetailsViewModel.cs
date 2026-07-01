using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;
using Android_Ispit.Services;

namespace Android_Ispit.ViewModels
{
    public partial class ProductDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ProductDTO? _product;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasRelatedProducts))]
        private ObservableCollection<ProductDTO> _relatedProducts = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _cartMessage = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanIncrementQuantity))]
        [NotifyPropertyChangedFor(nameof(CanDecrementQuantity))]
        private int _quantity = 1;

        public bool HasCartMessage => !string.IsNullOrEmpty(CartMessage);
        partial void OnCartMessageChanged(string value) => OnPropertyChanged(nameof(HasCartMessage));
        partial void OnProductChanged(ProductDTO? value) => Quantity = 1;

        public bool HasRelatedProducts => RelatedProducts.Count > 0;
        public bool CanIncrementQuantity => Quantity < CartStorage.MaxQuantityPerProduct;
        public bool CanDecrementQuantity => Quantity > 1;

        public async Task LoadRelatedAsync()
        {
            if (Product == null)
                return;

            IsBusy = true;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                RestRequest request = new RestRequest($"/products/{Product.Id}/related", Method.Get);
                var result = await client.ExecuteAsync<List<ProductDTO>>(request);
                if (result.IsSuccessful && result.Data != null)
                {
                    RelatedProducts = new ObservableCollection<ProductDTO>(result.Data);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void IncrementQuantity()
        {
            if (Quantity < CartStorage.MaxQuantityPerProduct)
                Quantity++;
        }

        [RelayCommand]
        private void DecrementQuantity()
        {
            if (Quantity > 1)
                Quantity--;
        }

        [RelayCommand]
        private async Task AddToCartAsync()
        {
            if (Product == null)
                return;

            await CartStorage.AddToCartAsync(Product, Quantity);
            CartMessage = $"Added {Quantity} to cart!";
            Quantity = 1;
        }
    }
}
