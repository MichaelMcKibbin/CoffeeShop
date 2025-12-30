using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using CoffeeShop.PageModels;
using CoffeeShop.Pages;
using CoffeeShop.Services;

namespace CoffeeShop;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        // Services
        builder.Services.AddSingleton<MenuService>();
        builder.Services.AddSingleton<BasketService>();
        builder.Services.AddSingleton<OrderStore>();

        // ViewModels (PageModels)
        builder.Services.AddTransient<LoginPageModel>();
        builder.Services.AddTransient<CategoriesPageModel>();
        builder.Services.AddTransient<MenuPageModel>();
        builder.Services.AddTransient<CheckoutPageModel>();
        builder.Services.AddTransient<ReceiptPageModel>();
        builder.Services.AddTransient<HistoryPageModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<CategoriesPage>();
        builder.Services.AddTransient<MenuPage>();
        builder.Services.AddTransient<CheckoutPage>();
        builder.Services.AddTransient<ReceiptPage>();
        builder.Services.AddTransient<HistoryPage>();

        return builder.Build();
    }
}
