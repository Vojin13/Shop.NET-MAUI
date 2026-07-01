using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Android_Ispit.DTO;

namespace Android_Ispit.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _role = string.Empty;

        [ObservableProperty]
        private string _avatar = string.Empty;

        [ObservableProperty]
        private string _createdDisplay = string.Empty;

        public async Task LoadAsync()
        {
            string? json = await SecureStorage.Default.GetAsync("user");
            if (string.IsNullOrEmpty(json))
                return;

            UserDTO? user = JsonConvert.DeserializeObject<UserDTO>(json);
            if (user == null)
                return;

            Email = user.Email;
            Role = user.Role;
            Avatar = user.Avatar;
            CreatedDisplay = user.CreationAt == default
                ? string.Empty
                : user.CreationAt.ToLocalTime().ToString("MMMM d, yyyy");
        }
    }
}
