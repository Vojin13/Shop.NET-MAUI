using Android_Ispit.DTO;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class UserEditPage : ContentPage
    {
        private readonly UserEditViewModel _viewModel;

        public UserEditPage(UserEditViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        public void PrepareForCreate()
        {
            Title = "New User";
            _viewModel.SetForCreate();
        }

        public void PrepareForEdit(UserDTO user)
        {
            Title = "Edit User";
            _viewModel.SetForEdit(user);
        }

        private async void OnSaveClicked(object? sender, EventArgs e)
        {
            bool success = await _viewModel.SaveAsync();
            if (success)
            {
                await Navigation.PopAsync();
            }
        }
    }
}
