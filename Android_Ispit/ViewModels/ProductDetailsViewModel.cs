using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;
using Android_Ispit.Services;

namespace Android_Ispit.ViewModels
{
    public partial class CarouselImageItem : ObservableObject
    {
        public string Url { get; set; } = string.Empty;
        public int Index { get; set; }

        [ObservableProperty]
        private bool _isActive;
    }

    public partial class ProductDetailsViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentImageUrl))]
        private ProductDTO? _product;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasRelatedProducts))]
        private ObservableCollection<ProductDTO> _relatedProducts = new();

        [ObservableProperty]
        private List<CarouselImageItem> _carouselImages = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentImageUrl))]
        private int _currentImageIndex;

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

        // No CarouselView here on purpose - it caused a persistent visual glitch (continuous
        // jitter) on the Windows Machine target. This is a plain Image whose Source follows
        // CurrentImageIndex, with tap zones/dots to change it - no native swipe control involved.
        public string CurrentImageUrl =>
            Product != null && Product.Images.Count > 0
                ? Product.Images[Math.Clamp(CurrentImageIndex, 0, Product.Images.Count - 1)]
                : Product?.MainImage ?? string.Empty;

        partial void OnProductChanged(ProductDTO? value)
        {
            Quantity = 1;
            CurrentImageIndex = 0;
            CarouselImages = (value?.Images ?? new List<string>())
                .Select((url, i) => new CarouselImageItem { Url = url, Index = i, IsActive = i == 0 })
                .ToList();
        }

        partial void OnCurrentImageIndexChanged(int value)
        {
            foreach (var item in CarouselImages)
            {
                item.IsActive = item.Index == value;
            }
        }

        public bool HasRelatedProducts => RelatedProducts.Count > 0;

        // Quantity is clamped both here (CanExecute gates the command itself) and via the
        // disabled Button state - relying on the button's IsEnabled alone wasn't enough:
        // a stray repeated command invocation could still push Quantity past the 1-10 bounds.
        public bool CanIncrementQuantity => Quantity < CartStorage.MaxQuantityPerProduct;
        public bool CanDecrementQuantity => Quantity > 1;

        partial void OnQuantityChanged(int value)
        {
            IncrementQuantityCommand.NotifyCanExecuteChanged();
            DecrementQuantityCommand.NotifyCanExecuteChanged();
        }

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
        private void SelectImage(int index)
        {
            if (Product == null || Product.Images.Count == 0)
                return;

            CurrentImageIndex = Math.Clamp(index, 0, Product.Images.Count - 1);
        }

        [RelayCommand]
        private void PreviousImage()
        {
            if (Product == null || Product.Images.Count == 0)
                return;

            CurrentImageIndex = CurrentImageIndex <= 0 ? Product.Images.Count - 1 : CurrentImageIndex - 1;
        }

        [RelayCommand]
        private void NextImage()
        {
            if (Product == null || Product.Images.Count == 0)
                return;

            CurrentImageIndex = CurrentImageIndex >= Product.Images.Count - 1 ? 0 : CurrentImageIndex + 1;
        }

        [RelayCommand(CanExecute = nameof(CanIncrementQuantity))]
        private void IncrementQuantity()
        {
            Quantity++;
        }

        [RelayCommand(CanExecute = nameof(CanDecrementQuantity))]
        private void DecrementQuantity()
        {
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
