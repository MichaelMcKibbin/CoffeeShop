using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels;

[QueryProperty(nameof(Order), "Order")]
public partial class ReceiptPageModel : BaseViewModel
{
    [ObservableProperty]
    private Order order = new();

    partial void OnOrderChanged(Order value)
    {
        Title = $"Order #{value.OrderNumber}";
    }

    [RelayCommand]
    private async Task BackToCategoriesAsync()
    {
        await Shell.Current.GoToAsync("//categories");
    }
}

