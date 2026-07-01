using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NameError))]
        [NotifyPropertyChangedFor(nameof(HasNameError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string _name = string.Empty;

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
        private string _successMessage = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        private bool _nameTouched;
        private bool _emailTouched;
        private bool _passwordTouched;

        public string NameError => string.IsNullOrWhiteSpace(Name) ? "Name is required." : "";
        public bool HasNameError => _nameTouched && !string.IsNullOrEmpty(NameError);

        public string EmailError => string.IsNullOrWhiteSpace(Email) ? "Email is required." : "";
        public bool HasEmailError => _emailTouched && !string.IsNullOrEmpty(EmailError);

        public string PasswordError => string.IsNullOrWhiteSpace(Password) || Password.Length < 5
            ? "Password must be at least 5 characters."
            : "";
        public bool HasPasswordError => _passwordTouched && !string.IsNullOrEmpty(PasswordError);

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

        public bool IsFormValid => NameError == "" && EmailError == "" && PasswordError == "";

        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));
        partial void OnSuccessMessageChanged(string value) => OnPropertyChanged(nameof(HasSuccessMessage));
        partial void OnNameChanged(string value) => _nameTouched = true;
        partial void OnEmailChanged(string value) => _emailTouched = true;
        partial void OnPasswordChanged(string value) => _passwordTouched = true;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            ApiError = string.Empty;
            SuccessMessage = string.Empty;
            IsBusy = true;
            try
            {
                RestClient client = new RestClient("http://localhost:3001/api/v1");
                RestRequest request = new RestRequest("/users", Method.Post);
                request.AddJsonBody(new
                {
                    email = Email,
                    name = Name,
                    password = Password,
                    role = "customer",
                    avatar = "https://i.pravatar.cc/150?u=" + Uri.EscapeDataString(Email)
                });

                var result = await client.ExecuteAsync(request);
                if (!result.IsSuccessful)
                {
                    ApiError = ApiErrorHelper.Parse(result.Content);
                    return;
                }

                SuccessMessage = "Registration successful! Log in on the Login tab.";
                Name = string.Empty;
                Email = string.Empty;
                Password = string.Empty;
                _nameTouched = false;
                _emailTouched = false;
                _passwordTouched = false;
                OnPropertyChanged(nameof(HasNameError));
                OnPropertyChanged(nameof(HasEmailError));
                OnPropertyChanged(nameof(HasPasswordError));
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
