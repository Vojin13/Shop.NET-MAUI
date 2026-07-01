using CommunityToolkit.Mvvm.ComponentModel;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class CategoryEditViewModel : ObservableObject
    {
        private int _editingCategoryId;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NameError))]
        [NotifyPropertyChangedFor(nameof(HasNameError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _name = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ImageUrlError))]
        [NotifyPropertyChangedFor(nameof(HasImageUrlError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _imageUrl = string.Empty;

        [ObservableProperty]
        private string _apiError = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        private bool _nameTouched;
        private bool _imageUrlTouched;

        public string NameError => string.IsNullOrWhiteSpace(Name) ? "Name is required." : "";
        public bool HasNameError => _nameTouched && !string.IsNullOrEmpty(NameError);

        public string ImageUrlError => string.IsNullOrWhiteSpace(ImageUrl) ? "Image URL is required." : "";
        public bool HasImageUrlError => _imageUrlTouched && !string.IsNullOrEmpty(ImageUrlError);

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));
        partial void OnNameChanged(string value) => _nameTouched = true;
        partial void OnImageUrlChanged(string value) => _imageUrlTouched = true;

        public bool IsFormValid => NameError == "" && ImageUrlError == "";

        public void SetForCreate()
        {
            _editingCategoryId = 0;
            IsEditMode = false;
            Name = string.Empty;
            ImageUrl = string.Empty;

            _nameTouched = false;
            _imageUrlTouched = false;
            OnPropertyChanged(nameof(HasNameError));
            OnPropertyChanged(nameof(HasImageUrlError));
        }

        public void SetForEdit(CategoryDTO category)
        {
            _editingCategoryId = category.Id;
            IsEditMode = true;
            Name = category.Name;
            ImageUrl = category.Image;
        }

        public async Task<bool> SaveAsync()
        {
            if (!IsFormValid)
                return false;

            ApiError = string.Empty;
            IsBusy = true;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                var body = new { name = Name, image = ImageUrl };

                RestRequest request = IsEditMode
                    ? new RestRequest($"/categories/{_editingCategoryId}", Method.Put)
                    : new RestRequest("/categories", Method.Post);
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
