using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class CartPage : ContentPage
    {
        private readonly CartViewModel _viewModel;

        public CartPage(CartViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadCartAsync();
        }
    }
}
