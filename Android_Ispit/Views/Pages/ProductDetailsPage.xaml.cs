using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.DTO;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class ProductDetailsPage : ContentPage
    {
        private readonly ProductDetailsViewModel _viewModel;
        private readonly IServiceProvider _services;

        public ProductDetailsPage(ProductDetailsViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _services = services;
            BindingContext = _viewModel;
        }

        public async Task ShowProductAsync(ProductDTO product)
        {
            _viewModel.Product = product;
            _viewModel.CartMessage = string.Empty;
            await _viewModel.LoadRelatedAsync();
        }

        private async void OnRelatedProductTapped(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not ProductDTO product)
                return;

            var detailsPage = _services.GetRequiredService<ProductDetailsPage>();
            await detailsPage.ShowProductAsync(product);
            await Navigation.PushAsync(detailsPage);
        }

        private async void OnBackTapped(object? sender, TappedEventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
