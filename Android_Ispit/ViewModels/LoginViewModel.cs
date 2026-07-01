using System.IdentityModel.Tokens.Jwt;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RestSharp;
using Android_Ispit.DTO;
using Android_Ispit.Views;

namespace Android_Ispit.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IServiceProvider _services;

        public LoginViewModel(IServiceProvider services)
        {
            _services = services;
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EmailError))]
        [NotifyPropertyChangedFor(nameof(HasEmailError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _email = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PasswordError))]
        [NotifyPropertyChangedFor(nameof(HasPasswordError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _apiError = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        private bool _emailTouched;
        private bool _passwordTouched;

        public string EmailError => string.IsNullOrWhiteSpace(Email) ? "Email is required." : "";
        public bool HasEmailError => _emailTouched && !string.IsNullOrEmpty(EmailError);

        public string PasswordError => string.IsNullOrWhiteSpace(Password) ? "Password is required." : "";
        public bool HasPasswordError => _passwordTouched && !string.IsNullOrEmpty(PasswordError);

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);

        public bool IsFormValid => EmailError == "" && PasswordError == "";

        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));
        partial void OnEmailChanged(string value) => _emailTouched = true;
        partial void OnPasswordChanged(string value) => _passwordTouched = true;

        [RelayCommand]
        public async Task LoginAsync()
        {
            ApiError = string.Empty;
            IsBusy = true;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");

                RestRequest loginRequest = new RestRequest("/auth/login", Method.Post);
                loginRequest.AddJsonBody(new { email = Email, password = Password });
                var loginResult = await client.ExecuteAsync(loginRequest);

                if (!loginResult.IsSuccessful || string.IsNullOrEmpty(loginResult.Content))
                {
                    ApiError = loginResult.StatusCode == HttpStatusCode.Unauthorized
                        ? "Invalid credentials."
                        : ApiErrorHelper.Parse(loginResult.Content);
                    return;
                }

                var tokenPair = JsonConvert.DeserializeObject<TokenPairDTO>(loginResult.Content);
                if (tokenPair == null || string.IsNullOrEmpty(tokenPair.AccessToken))
                {
                    ApiError = "Login failed.";
                    return;
                }

                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(tokenPair.AccessToken);

                RestRequest profileRequest = new RestRequest("/auth/profile", Method.Get);
                profileRequest.AddHeader("Authorization", "Bearer " + tokenPair.AccessToken);
                var profileResult = await client.ExecuteAsync<UserDTO>(profileRequest);

                if (!profileResult.IsSuccessful || profileResult.Data == null)
                {
                    ApiError = ApiErrorHelper.Parse(profileResult.Content);
                    return;
                }

                UserDTO user = profileResult.Data;
                user.Token = tokenPair.AccessToken;
                user.RefreshToken = tokenPair.RefreshToken;
                user.TokenExpiry = jwt.ValidTo.ToLocalTime();

                await SecureStorage.Default.SetAsync("user", JsonConvert.SerializeObject(user));

                if (Application.Current == null)
                    return;

                Application.Current.Windows[0].Page = user.IsAdmin
                    ? _services.GetRequiredService<AdminTabbedPage>()
                    : _services.GetRequiredService<CustomerTabbedPage>();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
