using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private const int PageSize = 10;
        private int _currentOffset;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private ObservableCollection<CategoryDTO> _categories = new();

        [ObservableProperty]
        private CategoryDTO? _selectedCategory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasNoResults))]
        private ObservableCollection<ProductDTO> _products = new();

        [ObservableProperty]
        private bool _isBusy;

        // Separate from IsBusy on purpose: RefreshView.IsRefreshing auto-invokes its Command whenever
        // it becomes true, regardless of binding direction. If this were shared with IsBusy, every
        // Next/Previous/Search fetch would also re-trigger LoadProductsAsync and reset the page to 0.
        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _apiError = string.Empty;

        [ObservableProperty]
        private int _currentPageNumber = 1;

        [ObservableProperty]
        private bool _canGoNext;

        [ObservableProperty]
        private bool _canGoPrevious;

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));

        public bool HasNoResults => !IsBusy && Products.Count == 0;
        partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(HasNoResults));

        public async Task LoadCategoriesAsync()
        {
            RestClient client = new RestClient("http://localhost:3001/api/v1");
            RestRequest request = new RestRequest("/categories", Method.Get);
            request.AddParameter("limit", 100);

            var result = await client.ExecuteAsync<List<CategoryDTO>>(request);
            if (result.IsSuccessful && result.Data != null)
            {
                var all = new List<CategoryDTO> { new CategoryDTO { Id = 0, Name = "All Categories" } };
                all.AddRange(result.Data);
                Categories = new ObservableCollection<CategoryDTO>(all);
                SelectedCategory = Categories[0];
            }
        }

        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            _currentOffset = 0;
            IsRefreshing = true;
            try
            {
                await FetchAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task SearchAsync()
        {
            _currentOffset = 0;
            await FetchAsync();
        }

        public async Task ApplyCategoryFilterAsync()
        {
            _currentOffset = 0;
            await FetchAsync();
        }

        [RelayCommand]
        public async Task NextPageAsync()
        {
            _currentOffset += PageSize;
            await FetchAsync();
        }

        [RelayCommand]
        public async Task PreviousPageAsync()
        {
            _currentOffset = Math.Max(0, _currentOffset - PageSize);
            await FetchAsync();
        }

        private async Task FetchAsync()
        {
            IsBusy = true;
            ApiError = string.Empty;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                RestRequest request = new RestRequest("/products", Method.Get);
                request.AddParameter("limit", PageSize);
                request.AddParameter("offset", _currentOffset);
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    request.AddParameter("title", SearchText);
                }
                if (SelectedCategory != null && SelectedCategory.Id != 0)
                {
                    request.AddParameter("categoryId", SelectedCategory.Id);
                }

                var result = await client.ExecuteAsync<List<ProductDTO>>(request);
                if (result.IsSuccessful && result.Data != null)
                {
                    Products = new ObservableCollection<ProductDTO>(result.Data);
                    CanGoNext = result.Data.Count == PageSize;
                    CanGoPrevious = _currentOffset > 0;
                    CurrentPageNumber = (_currentOffset / PageSize) + 1;
                }
                else
                {
                    ApiError = ApiErrorHelper.Parse(result.Content);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task DeleteProductAsync(ProductDTO product)
        {
            if (product == null)
                return;

            RestClient client = new RestClient("http://localhost:3001/api/v1");
            RestRequest request = new RestRequest($"/products/{product.Id}", Method.Delete);
            var result = await client.ExecuteAsync(request);

            if (result.IsSuccessful)
            {
                Products.Remove(product);
            }
            else
            {
                ApiError = ApiErrorHelper.Parse(result.Content);
            }
        }
    }
}
