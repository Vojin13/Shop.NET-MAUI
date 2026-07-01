using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class ProductEditViewModel : ObservableObject
    {
        private int _editingProductId;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TitleError))]
        [NotifyPropertyChangedFor(nameof(HasTitleError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _title = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PriceError))]
        [NotifyPropertyChangedFor(nameof(HasPriceError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _priceText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DescriptionError))]
        [NotifyPropertyChangedFor(nameof(HasDescriptionError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _description = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImagesError))]
        [NotifyPropertyChangedFor(nameof(HasImagesError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _imagesText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CategoryError))]
        [NotifyPropertyChangedFor(nameof(HasCategoryError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private CategoryDTO? _selectedCategory;

        [ObservableProperty]
        private ObservableCollection<CategoryDTO> _categories = new();

        [ObservableProperty]
        private string _apiError = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        private bool _titleTouched;
        private bool _priceTouched;
        private bool _descriptionTouched;
        private bool _imagesTouched;
        private bool _categoryTouched;

        public string TitleError => string.IsNullOrWhiteSpace(Title) ? "Title is required." : "";
        public bool HasTitleError => _titleTouched && !string.IsNullOrEmpty(TitleError);

        public string PriceError => !decimal.TryParse(PriceText, out var price) || price <= 0
            ? "Price must be a positive number."
            : "";
        public bool HasPriceError => _priceTouched && !string.IsNullOrEmpty(PriceError);

        public string DescriptionError => string.IsNullOrWhiteSpace(Description) ? "Description is required." : "";
        public bool HasDescriptionError => _descriptionTouched && !string.IsNullOrEmpty(DescriptionError);

        public string ImagesError => ParseImages().Count == 0
            ? "Enter at least one image URL (one per line)."
            : "";
        public bool HasImagesError => _imagesTouched && !string.IsNullOrEmpty(ImagesError);

        public string CategoryError => SelectedCategory == null ? "Select a category." : "";
        public bool HasCategoryError => _categoryTouched && !string.IsNullOrEmpty(CategoryError);

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));
        partial void OnTitleChanged(string value) => _titleTouched = true;
        partial void OnPriceTextChanged(string value) => _priceTouched = true;
        partial void OnDescriptionChanged(string value) => _descriptionTouched = true;
        partial void OnImagesTextChanged(string value) => _imagesTouched = true;
        partial void OnSelectedCategoryChanged(CategoryDTO? value) => _categoryTouched = true;

        public bool IsFormValid =>
            TitleError == "" && PriceError == "" && DescriptionError == "" &&
            ImagesError == "" && CategoryError == "";

        // The Editor's line breaks aren't reliably "\n" across platforms - on Windows Machine it's
        // been observed to report bare "\r" with no "\n" at all, which a Split('\n') silently
        // fails to split on. Splitting on all three line-ending styles handles this everywhere.
        private static readonly string[] LineBreakSeparators = { "\r\n", "\r", "\n" };

        private List<string> ParseImages()
        {
            return ImagesText
                .Split(LineBreakSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .ToList();
        }

        public void SetForCreate()
        {
            _editingProductId = 0;
            IsEditMode = false;
            Title = string.Empty;
            PriceText = string.Empty;
            Description = string.Empty;
            ImagesText = string.Empty;
            SelectedCategory = null;

            _titleTouched = false;
            _priceTouched = false;
            _descriptionTouched = false;
            _imagesTouched = false;
            _categoryTouched = false;
            OnPropertyChanged(nameof(HasTitleError));
            OnPropertyChanged(nameof(HasPriceError));
            OnPropertyChanged(nameof(HasDescriptionError));
            OnPropertyChanged(nameof(HasImagesError));
            OnPropertyChanged(nameof(HasCategoryError));
        }

        public void SetForEdit(ProductDTO product)
        {
            _editingProductId = product.Id;
            IsEditMode = true;
            Title = product.Title;
            PriceText = product.Price.ToString();
            Description = product.Description;
            ImagesText = string.Join("\n", product.Images);
        }

        public async Task LoadCategoriesAsync()
        {
            RestClient client = new RestClient("http://localhost:3001/api/v1");
            RestRequest request = new RestRequest("/categories", Method.Get);
            request.AddParameter("limit", 100);

            var result = await client.ExecuteAsync<List<CategoryDTO>>(request);
            if (result.IsSuccessful && result.Data != null)
            {
                Categories = new ObservableCollection<CategoryDTO>(result.Data);
            }
        }

        public int? CategoryIdToPreselect { get; set; }

        public void ApplyPreselectedCategory()
        {
            if (CategoryIdToPreselect.HasValue)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == CategoryIdToPreselect.Value);
            }
        }

        public async Task<bool> SaveAsync()
        {
            if (!IsFormValid || SelectedCategory == null)
                return false;

            ApiError = string.Empty;
            IsBusy = true;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                var body = new
                {
                    title = Title,
                    price = decimal.Parse(PriceText),
                    description = Description,
                    categoryId = SelectedCategory.Id,
                    images = ParseImages()
                };

                RestRequest request = IsEditMode
                    ? new RestRequest($"/products/{_editingProductId}", Method.Put)
                    : new RestRequest("/products", Method.Post);
                request.AddJsonBody(body);

                var result = await client.ExecuteAsync(request);
                if (result.IsSuccessful)
                    return true;

                ApiError = ApiErrorHelper.Parse(result.Content);
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
