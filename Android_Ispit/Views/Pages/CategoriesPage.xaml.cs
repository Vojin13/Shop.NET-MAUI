using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.DTO;
using Android_Ispit.Services;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class CategoriesPage : ContentPage
    {
        private readonly CategoriesViewModel _viewModel;
        private readonly IServiceProvider _services;

        public CategoriesPage(CategoriesViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _services = services;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCategoriesCommand.ExecuteAsync(null);
        }

        private void OnSearchButtonPressed(object? sender, EventArgs e)
        {
            _viewModel.SearchCommand.Execute(null);
        }

        private async void OnAddClicked(object? sender, EventArgs e)
        {
            var editPage = _services.GetRequiredService<CategoryEditPage>();
            editPage.PrepareForCreate();
            await Navigation.PushAsync(editPage);
        }

        private async void OnEditClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not CategoryDTO category)
                return;

            var editPage = _services.GetRequiredService<CategoryEditPage>();
            editPage.PrepareForEdit(category);
            await Navigation.PushAsync(editPage);
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not CategoryDTO category)
                return;

            bool confirmed = await DisplayAlertAsync("Delete Category", $"Are you sure you want to delete \"{category.Name}\"?", "Yes", "No");
            if (!confirmed)
                return;

            await _viewModel.DeleteCategoryCommand.ExecuteAsync(category);
        }

        private void OnLogoutTapped(object? sender, TappedEventArgs e)
        {
            SecureStorage.Default.Remove("user");
            CartStorage.ClearCart();
            if (Application.Current != null)
                Application.Current.Windows[0].Page = _services.GetRequiredService<AuthPage>();
        }

        private async void OnProfileTapped(object? sender, TappedEventArgs e)
        {
            var profilePage = _services.GetRequiredService<ProfilePage>();
            await Navigation.PushAsync(profilePage);
        }
    }
}
