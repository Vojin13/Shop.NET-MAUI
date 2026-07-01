using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class CategoriesViewModel : ObservableObject
    {
        // The API has no name-search query param for categories, so search is done locally
        // over this full list instead of round-tripping to the server per keystroke/submit.
        private List<CategoryDTO> _allCategories = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoResults))]
        [NotifyPropertyChangedFor(nameof(ItemCountText))]
        private ObservableCollection<CategoryDTO> _categories = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _apiError = string.Empty;

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));

        public bool HasNoResults => !IsBusy && Categories.Count == 0;
        partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(HasNoResults));

        public string ItemCountText => $"{Categories.Count} categor{(Categories.Count == 1 ? "y" : "ies")}";

        [RelayCommand]
        public async Task LoadCategoriesAsync()
        {
            IsBusy = true;
            IsRefreshing = true;
            ApiError = string.Empty;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                RestRequest request = new RestRequest("/categories", Method.Get);
                request.AddParameter("limit", 100);

                var result = await client.ExecuteAsync<List<CategoryDTO>>(request);
                if (result.IsSuccessful && result.Data != null)
                {
                    _allCategories = result.Data;
                    ApplyFilter();
                }
                else
                {
                    ApiError = ApiErrorHelper.Parse(result.Content);
                }
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public void Search()
        {
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _allCategories
                : _allCategories.Where(c => c.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            Categories = new ObservableCollection<CategoryDTO>(filtered);
        }

        [RelayCommand]
        private async Task DeleteCategoryAsync(CategoryDTO category)
        {
            if (category == null)
                return;

            RestClient client = new RestClient("http://localhost:3001/api/v1");
            RestRequest request = new RestRequest($"/categories/{category.Id}", Method.Delete);
            var result = await client.ExecuteAsync(request);

            if (result.IsSuccessful)
            {
                _allCategories.Remove(category);
                ApplyFilter();
            }
            else
            {
                ApiError = ApiErrorHelper.Parse(result.Content);
            }
        }
    }
}
