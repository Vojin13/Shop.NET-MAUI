using Android_Ispit.DTO;
using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class CategoryEditPage : ContentPage
    {
        private readonly CategoryEditViewModel _viewModel;

        public CategoryEditPage(CategoryEditViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        public void PrepareForCreate()
        {
            Title = "New Category";
            _viewModel.SetForCreate();
        }

        public void PrepareForEdit(CategoryDTO category)
        {
            Title = "Edit Category";
            _viewModel.SetForEdit(category);
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
