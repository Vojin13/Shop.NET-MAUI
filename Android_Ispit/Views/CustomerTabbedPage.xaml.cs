using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Android_Ispit.Views.Pages;

namespace Android_Ispit.Views
{
    public partial class CustomerTabbedPage : Microsoft.Maui.Controls.TabbedPage
    {
        public CustomerTabbedPage(ShopPage shopPage)
        {
            InitializeComponent();

            shopPage.Title = "Shop";

            Children.Add(new NavigationPage(shopPage) { Title = "Shop" });

            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(false);
            On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
        }
    }
}
