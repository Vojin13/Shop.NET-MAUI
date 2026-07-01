using Microsoft.Extensions.DependencyInjection;
using Android_Ispit.Views;

namespace Android_Ispit
{
    public partial class App : Application
    {
        private readonly IServiceProvider _services;

        public App(IServiceProvider services)
        {
            InitializeComponent();
            _services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(_services.GetRequiredService<AuthTabbedPage>())
            {
                Width = 390,
                Height = 850
            };
            return window;
        }
    }
}