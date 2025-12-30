using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CoffeeShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace CoffeeShop.PageModels;

[QueryProperty(nameof(CategoryName), "Category")]
public partial class MenuPageModel : BaseViewModel
{
    private readonly MenuService _menu;
    private readonly BasketService _basket;

    public MenuPageModel(MenuService menu, BasketService basket)
    {
        _menu = menu;
        _basket = basket;
        Items = new ObservableCollection<CoffeeShopMenuItem>();
    }

    public ObservableCollection<CoffeeShopMenuItem> Items { get; }

    private string? _categoryName;
    public string? CategoryName
    {
        get => _categoryName;
        set
        {
            _categoryName = value;

            if (!Enum.TryParse<MenuCategory>(value, out var parsed))
                parsed = MenuCategory.Food;

            Title = parsed.ToString();

            Items.Clear();
            foreach (var item in _menu.GetByCategory(parsed))
                Items.Add(item);

            OnPropertyChanged(nameof(BasketTotal));
        }
    }

    public decimal BasketTotal => _basket.Total;

    [RelayCommand]
    private void Add(CoffeeShopMenuItem item)
    {
        _basket.Add(item);
        OnPropertyChanged(nameof(BasketTotal));
    }

    [RelayCommand]
    private void Decrease(CoffeeShopMenuItem item)
    {
        _basket.Decrease(item);
        OnPropertyChanged(nameof(BasketTotal));
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        await Shell.Current.GoToAsync("checkout");
    }
}