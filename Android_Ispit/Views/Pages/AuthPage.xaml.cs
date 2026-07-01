using Android_Ispit.ViewModels;

namespace Android_Ispit.Views.Pages
{
    public partial class AuthPage : ContentPage
    {
        private readonly LoginViewModel _loginViewModel;
        private readonly RegisterViewModel _registerViewModel;

        public AuthPage(LoginViewModel loginViewModel, RegisterViewModel registerViewModel)
        {
            InitializeComponent();
            _loginViewModel = loginViewModel;
            _registerViewModel = registerViewModel;
            LoginSection.BindingContext = _loginViewModel;
            RegisterSection.BindingContext = _registerViewModel;
            ShowLogin();
        }

        private void OnLoginTabClicked(object? sender, EventArgs e) => ShowLogin();

        private void OnRegisterTabClicked(object? sender, EventArgs e) => ShowRegister();

        // Entry.Completed fires on Enter/Done from the keyboard - mirrors the single button's
        // IsEnabled gate (IsFormValid) rather than the command's own CanExecute, since neither
        // LoginCommand nor RegisterCommand has a CanExecute gate of its own.
        private void OnLoginEntryCompleted(object? sender, EventArgs e)
        {
            if (_loginViewModel.IsFormValid)
                _loginViewModel.LoginCommand.Execute(null);
        }

        private void OnRegisterEntryCompleted(object? sender, EventArgs e)
        {
            if (_registerViewModel.IsFormValid)
                _registerViewModel.RegisterCommand.Execute(null);
        }

        private void ShowLogin()
        {
            LoginSection.IsVisible = true;
            RegisterSection.IsVisible = false;
            FooterLink.IsVisible = false;
            HeadlineLabel.Text = "Welcome back";
            SubtitleLabel.Text = "Log in to keep shopping.";

            LoginTabButton.BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"];
            LoginTabButton.TextColor = (Color)Application.Current!.Resources["Ink"];
            LoginTabButton.FontAttributes = FontAttributes.Bold;

            RegisterTabButton.BackgroundColor = Colors.Transparent;
            RegisterTabButton.TextColor = (Color)Application.Current!.Resources["InkMuted"];
            RegisterTabButton.FontAttributes = FontAttributes.None;
        }

        private void ShowRegister()
        {
            LoginSection.IsVisible = false;
            RegisterSection.IsVisible = true;
            FooterLink.IsVisible = true;
            HeadlineLabel.Text = "Create account";
            SubtitleLabel.Text = "Join in a few seconds.";

            RegisterTabButton.BackgroundColor = (Color)Application.Current!.Resources["SurfaceCard"];
            RegisterTabButton.TextColor = (Color)Application.Current!.Resources["Ink"];
            RegisterTabButton.FontAttributes = FontAttributes.Bold;

            LoginTabButton.BackgroundColor = Colors.Transparent;
            LoginTabButton.TextColor = (Color)Application.Current!.Resources["InkMuted"];
            LoginTabButton.FontAttributes = FontAttributes.None;
        }
    }
}
