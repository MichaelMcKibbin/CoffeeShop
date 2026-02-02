using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

using CoffeeShop.Pages;

namespace CoffeeShop;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("menu", typeof(MenuPage));
        Routing.RegisterRoute("checkout", typeof(CheckoutPage));
        Routing.RegisterRoute("receipt", typeof(ReceiptPage));
        Routing.RegisterRoute("history", typeof(HistoryPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }
}
