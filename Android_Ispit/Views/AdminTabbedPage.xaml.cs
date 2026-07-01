using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Android_Ispit.Views.Pages;

namespace Android_Ispit.Views
{
    public partial class AdminTabbedPage : Microsoft.Maui.Controls.TabbedPage
    {
        public AdminTabbedPage(DashboardPage dashboardPage, UsersPage usersPage, CategoriesPage categoriesPage)
        {
            InitializeComponent();

            dashboardPage.Title = "Products";
            usersPage.Title = "Users";
            categoriesPage.Title = "Categories";

            Children.Add(new NavigationPage(dashboardPage) { Title = "Products" });
            Children.Add(new NavigationPage(usersPage) { Title = "Users" });
            Children.Add(new NavigationPage(categoriesPage) { Title = "Categories" });

            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
