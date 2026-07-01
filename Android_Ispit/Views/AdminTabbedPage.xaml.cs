using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Android_Ispit.Views.Pages;

namespace Android_Ispit.Views
{
    public partial class AdminTabbedPage : Microsoft.Maui.Controls.TabbedPage
    {
        public AdminTabbedPage(DashboardPage dashboardPage, UsersPage usersPage)
        {
            InitializeComponent();

            dashboardPage.Title = "Dashboard";
            usersPage.Title = "Users";

            Children.Add(new NavigationPage(dashboardPage) { Title = "Dashboard" });
            Children.Add(new NavigationPage(usersPage) { Title = "Users" });

            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
