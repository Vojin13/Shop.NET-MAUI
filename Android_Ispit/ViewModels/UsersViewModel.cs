using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<UserDTO> _users = new();

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _apiError = string.Empty;

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));

        [RelayCommand]
        public async Task LoadUsersAsync()
        {
            IsBusy = true;
            ApiError = string.Empty;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                RestRequest request = new RestRequest("/users", Method.Get);
                request.AddParameter("limit", 100);

                var result = await client.ExecuteAsync<List<UserDTO>>(request);
                if (result.IsSuccessful && result.Data != null)
                {
                    Users = new ObservableCollection<UserDTO>(result.Data.OrderBy(u => u.Id));
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
        private async Task DeleteUserAsync(UserDTO user)
        {
            if (user == null)
                return;

            RestClient client = new RestClient("http://localhost:3001/api/v1");
            RestRequest request = new RestRequest($"/users/{user.Id}", Method.Delete);
            var result = await client.ExecuteAsync(request);

            if (result.IsSuccessful)
            {
                Users.Remove(user);
            }
            else
            {
                ApiError = ApiErrorHelper.Parse(result.Content);
            }
        }
    }
}
