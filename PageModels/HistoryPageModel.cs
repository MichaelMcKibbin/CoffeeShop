using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CoffeeShop.Services;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CoffeeShop.PageModels;

public partial class HistoryPageModel : BaseViewModel
{
    private readonly OrderStore _store;

    public HistoryPageModel(OrderStore store)
    {
        _store = store;
        Title = "Order History";
        Orders = new ObservableCollection<Order>();
    }

    public ObservableCollection<Order> Orders { get; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Orders.Clear();
        var allOrders = await _store.LoadAllAsync();
        foreach (var o in allOrders) Orders.Add(o);
    }
}
