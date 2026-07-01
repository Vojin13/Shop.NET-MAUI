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
                    fonts.AddFont("SpaceGrotesk.ttf", "SpaceGrotesk");
                    fonts.AddFont("HankenGrotesk.ttf", "HankenGrotesk");
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
            builder.Services.AddTransient<CategoriesViewModel>();
            builder.Services.AddTransient<CategoryEditViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();

            // Pages
            builder.Services.AddTransient<AuthPage>();
            builder.Services.AddTransient<ShopPage>();
            builder.Services.AddTransient<ProductDetailsPage>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<ProductEditPage>();
            builder.Services.AddTransient<UsersPage>();
            builder.Services.AddTransient<UserEditPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<CategoriesPage>();
            builder.Services.AddTransient<CategoryEditPage>();
            builder.Services.AddTransient<ProfilePage>();

            // Tabbed root pages
            builder.Services.AddTransient<CustomerTabbedPage>();
            builder.Services.AddTransient<AdminTabbedPage>();

            return builder.Build();
        }
    }
}
