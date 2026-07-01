using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.DTO;
using Android_Ispit.Services;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class DashboardPage : ContentPage
    {
        private readonly DashboardViewModel _viewModel;
        private readonly IServiceProvider _services;

        public DashboardPage(DashboardViewModel viewModel, IServiceProvider services)
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

        private async void OnAddClicked(object? sender, EventArgs e)
        {
            var editPage = _services.GetRequiredService<ProductEditPage>();
            await editPage.PrepareForCreateAsync();
            await Navigation.PushAsync(editPage);
        }

        private async void OnEditClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not ProductDTO product)
                return;

            var editPage = _services.GetRequiredService<ProductEditPage>();
            await editPage.PrepareForEditAsync(product);
            await Navigation.PushAsync(editPage);
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not ProductDTO product)
                return;

            bool confirmed = await DisplayAlertAsync("Delete Product", $"Are you sure you want to delete \"{product.Title}\"?", "Yes", "No");
            if (!confirmed)
                return;

            await _viewModel.DeleteProductCommand.ExecuteAsync(product);
        }

        private void OnLogoutClicked(object? sender, EventArgs e)
        {
            SecureStorage.Default.Remove("user");
            CartStorage.ClearCart();
            if (Application.Current != null)
                Application.Current.Windows[0].Page = _services.GetRequiredService<AuthTabbedPage>();
        }
    }
}
