using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.DTO;
using Android_Ispit.Services;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class ShopPage : ContentPage
    {
        private readonly ShopViewModel _viewModel;
        private readonly IServiceProvider _services;

        public ShopPage(ShopViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _services = services;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCategoriesAsync();
            await _viewModel.LoadProductsCommand.ExecuteAsync(null);
            await _viewModel.RefreshCartCountAsync();
        }

        private async void OnCartTapped(object? sender, TappedEventArgs e)
        {
            var cartPage = _services.GetRequiredService<CartPage>();
            await Navigation.PushAsync(cartPage);
        }

        private async void OnCategoryChanged(object? sender, EventArgs e)
        {
            await _viewModel.ApplyCategoryFilterAsync();
            await ScrollToTopAsync();
        }

        private async void OnSearchButtonPressed(object? sender, EventArgs e)
        {
            await _viewModel.SearchCommand.ExecuteAsync(null);
            await ScrollToTopAsync();
        }

        private async void OnNextClicked(object? sender, EventArgs e)
        {
            await _viewModel.NextPageCommand.ExecuteAsync(null);
            await ScrollToTopAsync();
        }

        private async void OnPreviousClicked(object? sender, EventArgs e)
        {
            await _viewModel.PreviousPageCommand.ExecuteAsync(null);
            await ScrollToTopAsync();
        }

        private Task ScrollToTopAsync()
        {
            return ContentScrollView.ScrollToAsync(0, 0, false);
        }

        private async void OnProductTapped(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not ProductDTO product)
                return;

            var detailsPage = _services.GetRequiredService<ProductDetailsPage>();
            await detailsPage.ShowProductAsync(product);
            await Navigation.PushAsync(detailsPage);
        }

        private void OnLogoutTapped(object? sender, TappedEventArgs e)
        {
            SecureStorage.Default.Remove("user");
            CartStorage.ClearCart();
            if (Application.Current != null)
                Application.Current.Windows[0].Page = _services.GetRequiredService<AuthPage>();
        }
    }
}
