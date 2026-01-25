using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CoffeeShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CoffeeShop.PageModels;

public partial class CheckoutPageModel : BaseViewModel
{
    private readonly BasketService _basket;
    private readonly OrderStore _store;
    private readonly UserSession _userSession;

    public CheckoutPageModel(BasketService basket, OrderStore store, UserSession userSession)
    {
        _basket = basket;
        _store = store;
        _userSession = userSession;

        Lines = _basket.Lines; // bind directly
        Title = "Checkout";
        
        // Auto-populate from logged-in user
        if (_userSession.IsLoggedIn && _userSession.CurrentUser != null)
        {
            CustomerName = _userSession.CurrentUser.FullName;
            Telephone = _userSession.CurrentUser.PhoneNumber;
        }
    }

    public ObservableCollection<OrderLine> Lines { get; }

    public decimal Total => _basket.Total;

    [ObservableProperty] private string customerName = "";
    [ObservableProperty] private string telephone = "";

    [RelayCommand]
    private void RemoveLine(OrderLine line)
    {
        _basket.RemoveLine(line);
        OnPropertyChanged(nameof(Total));
    }

    [RelayCommand]
    private async Task PlaceOrderAsync()
    {
        if (Lines.Count == 0)
        {
            await Shell.Current.DisplayAlert("Empty basket", "Add at least one item.", "OK");
            return;
        }

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            LocalDateTime = DateTime.Now,
            CustomerName = CustomerName.Trim(),
            Telephone = Telephone.Trim(),
            Lines = _basket.SnapshotLines()
        };

        await _store.SaveOrderAsync(order);

        _basket.Clear();

        await Shell.Current.GoToAsync("receipt", new Dictionary<string, object>
        {
            ["Order"] = order
        });
    }

    private static string GenerateOrderNumber()
    {
        // short-ish unique number for a CA
        return DateTime.Now.ToString("yyyyMMdd-HHmmss");
    }
}

