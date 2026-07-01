using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Android_Ispit.Views.Pages;

namespace Android_Ispit.Views
{
    public partial class AuthTabbedPage : Microsoft.Maui.Controls.TabbedPage
    {
        public AuthTabbedPage(LoginPage loginPage, RegisterPage registerPage)
        {
            InitializeComponent();

            loginPage.Title = "Login";
            registerPage.Title = "Register";

            Children.Add(loginPage);
            Children.Add(registerPage);

            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
