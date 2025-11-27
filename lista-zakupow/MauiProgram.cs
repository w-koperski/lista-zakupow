using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;
using lista_zakupow.Views;
using lista_zakupow.ViewModels;

namespace lista_zakupow
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
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

#if DEBUG
                builder.Logging.AddDebug();
#endif
            // Register VM for DI (optional if using XAML new object)
            builder.Services.AddTransient<ShoppingListViewModel>();
            builder.Services.AddTransient<ShoppingListPage>();

            return builder.Build();
        }
    }
}
