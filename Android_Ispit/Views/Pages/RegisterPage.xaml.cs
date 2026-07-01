using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel _viewModel;

        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }
}
