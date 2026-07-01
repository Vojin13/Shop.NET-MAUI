using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using RestSharp;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class UserEditViewModel : ObservableObject
    {
        private int _editingUserId;
        private string _editingEmail = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PasswordPlaceholder))]
        private bool _isEditMode;

        public string PasswordPlaceholder => IsEditMode ? "Leave blank to keep current password" : "Password";

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
        [NotifyPropertyChangedFor(nameof(RoleError))]
        [NotifyPropertyChangedFor(nameof(HasRoleError))]
        [NotifyPropertyChangedFor(nameof(IsFormValid))]
        private string? _selectedRole;

        [ObservableProperty]
        private string _apiError = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        public List<string> Roles { get; } = new() { "customer", "admin" };

        private bool _nameTouched;
        private bool _emailTouched;
        private bool _passwordTouched;
        private bool _roleTouched;

        public string NameError => string.IsNullOrWhiteSpace(Name) ? "Name is required." : "";
        public bool HasNameError => _nameTouched && !string.IsNullOrEmpty(NameError);

        public string EmailError => string.IsNullOrWhiteSpace(Email) ? "Email is required." : "";
        public bool HasEmailError => _emailTouched && !string.IsNullOrEmpty(EmailError);

        public string PasswordError
        {
            get
            {
                if (!IsEditMode && string.IsNullOrWhiteSpace(Password))
                    return "Password is required.";
                if (!string.IsNullOrWhiteSpace(Password) &&
                    (Password.Length < 4 || !Password.All(char.IsLetterOrDigit)))
                    return "Password must be alphanumeric and at least 4 characters.";
                return "";
            }
        }
        public bool HasPasswordError => _passwordTouched && !string.IsNullOrEmpty(PasswordError);

        public string RoleError => string.IsNullOrEmpty(SelectedRole) ? "Select a role." : "";
        public bool HasRoleError => _roleTouched && !string.IsNullOrEmpty(RoleError);

        public bool HasApiError => !string.IsNullOrEmpty(ApiError);
        partial void OnApiErrorChanged(string value) => OnPropertyChanged(nameof(HasApiError));
        partial void OnNameChanged(string value) => _nameTouched = true;
        partial void OnEmailChanged(string value) => _emailTouched = true;
        partial void OnPasswordChanged(string value) => _passwordTouched = true;
        partial void OnSelectedRoleChanged(string? value) => _roleTouched = true;

        public bool IsFormValid =>
            NameError == "" && EmailError == "" && PasswordError == "" && RoleError == "";

        public void SetForCreate()
        {
            _editingUserId = 0;
            _editingEmail = string.Empty;
            IsEditMode = false;
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            SelectedRole = "customer";

            _nameTouched = false;
            _emailTouched = false;
            _passwordTouched = false;
            _roleTouched = false;
            OnPropertyChanged(nameof(HasNameError));
            OnPropertyChanged(nameof(HasEmailError));
            OnPropertyChanged(nameof(HasPasswordError));
            OnPropertyChanged(nameof(HasRoleError));
        }

        public void SetForEdit(UserDTO user)
        {
            _editingUserId = user.Id;
            _editingEmail = user.Email;
            IsEditMode = true;
            Name = user.Name;
            Email = user.Email;
            Password = string.Empty;
            SelectedRole = user.Role;
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
                string avatarSeed = string.IsNullOrEmpty(Email) ? _editingEmail : Email;

                if (IsEditMode)
                {
                    // Password is intentionally omitted (not sent as null) when left blank, so the
                    // API's partial merge leaves the stored password untouched instead of wiping it.
                    var body = new Dictionary<string, object>
                    {
                        ["name"] = Name,
                        ["email"] = Email,
                        ["role"] = SelectedRole!,
                        ["avatar"] = "https://i.pravatar.cc/150?u=" + Uri.EscapeDataString(avatarSeed)
                    };
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        body["password"] = Password;
                    }

                    RestRequest request = new RestRequest($"/users/{_editingUserId}", Method.Put);
                    request.AddJsonBody(body);
                    var result = await client.ExecuteAsync(request);
                    if (result.IsSuccessful)
                        return true;

                    ApiError = ApiErrorHelper.Parse(result.Content);
                    return false;
                }
                else
                {
                    var body = new
                    {
                        name = Name,
                        email = Email,
                        role = SelectedRole,
                        avatar = "https://i.pravatar.cc/150?u=" + Uri.EscapeDataString(avatarSeed),
                        password = Password
                    };
                    RestRequest request = new RestRequest("/users", Method.Post);
                    request.AddJsonBody(body);
                    var result = await client.ExecuteAsync(request);
                    if (result.IsSuccessful)
                        return true;

                    ApiError = ApiErrorHelper.Parse(result.Content);
                    return false;
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
