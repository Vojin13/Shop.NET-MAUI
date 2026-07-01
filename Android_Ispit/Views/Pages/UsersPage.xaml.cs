using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.DTO;
using Android_Ispit.Services;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class UsersPage : ContentPage
    {
        private readonly UsersViewModel _viewModel;
        private readonly IServiceProvider _services;

        public UsersPage(UsersViewModel viewModel, IServiceProvider services)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _services = services;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadUsersCommand.ExecuteAsync(null);
        }

        private async void OnAddClicked(object? sender, EventArgs e)
        {
            var editPage = _services.GetRequiredService<UserEditPage>();
            editPage.PrepareForCreate();
            await Navigation.PushAsync(editPage);
        }

        private async void OnEditClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not UserDTO user)
                return;

            var editPage = _services.GetRequiredService<UserEditPage>();
            editPage.PrepareForEdit(user);
            await Navigation.PushAsync(editPage);
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if ((sender as Element)?.BindingContext is not UserDTO user)
                return;

            bool confirmed = await DisplayAlertAsync("Delete User", $"Are you sure you want to delete \"{user.Name}\"?", "Yes", "No");
            if (!confirmed)
                return;

            await _viewModel.DeleteUserCommand.ExecuteAsync(user);
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
