using Android_Ispit.DTO;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class ProductEditPage : ContentPage
    {
        private readonly ProductEditViewModel _viewModel;

        public ProductEditPage(ProductEditViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        public async Task PrepareForCreateAsync()
        {
            Title = "New Product";
            _viewModel.SetForCreate();
            await _viewModel.LoadCategoriesAsync();
        }

        public async Task PrepareForEditAsync(ProductDTO product)
        {
            Title = "Edit Product";
            _viewModel.SetForEdit(product);
            _viewModel.CategoryIdToPreselect = product.Category?.Id;
            await _viewModel.LoadCategoriesAsync();
            _viewModel.ApplyPreselectedCategory();
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
