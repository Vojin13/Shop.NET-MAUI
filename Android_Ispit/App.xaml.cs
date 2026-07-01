using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.Views.Pages;

namespace Android_Ispit
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;

            // Nimbus visual direction is dark-only by design - force it regardless of system theme.
            UserAppTheme = AppTheme.Dark;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(_services.GetRequiredService<AuthPage>())
            {
                Width = 390,
                Height = 850
            };
            return window;
        }
    }
}