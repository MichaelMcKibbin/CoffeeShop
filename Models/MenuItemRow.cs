using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CoffeeShop.Models;

public partial class MenuItemRow : ObservableObject
{
    public CoffeeShopMenuItem Item { get; }

    public string Name => Item.Name;
    public decimal Price => Item.Price;

    [ObservableProperty]
    private int quantity;

    public MenuItemRow(CoffeeShopMenuItem item, int quantity = 0)
    {
        Item = item;
        this.quantity = quantity;
    }
}
