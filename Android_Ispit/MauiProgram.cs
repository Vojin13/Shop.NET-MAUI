using Microsoft.Extensions.Logging;
using Android_Ispit.ViewModels;
using Android_Ispit.Views;
using Android_Ispit.Views.Pages;

namespace Android_Ispit
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<ShopViewModel>();
            builder.Services.AddTransient<ProductDetailsViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<ProductEditViewModel>();
            builder.Services.AddTransient<UsersViewModel>();
            builder.Services.AddTransient<UserEditViewModel>();
            builder.Services.AddTransient<CartViewModel>();

            // Pages
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<ShopPage>();
            builder.Services.AddTransient<ProductDetailsPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ProductEditPage>();
            builder.Services.AddTransient<UsersPage>();
            builder.Services.AddTransient<UserEditPage>();
            builder.Services.AddTransient<CartPage>();

            // Tabbed root pages
            builder.Services.AddTransient<AuthTabbedPage>();
            builder.Services.AddTransient<CustomerTabbedPage>();
            builder.Services.AddTransient<AdminTabbedPage>();

            return builder.Build();
        }
    }
}
