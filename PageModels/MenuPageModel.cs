using System;
using System.Linq;
using CoffeeShop.Models;
using CoffeeShop.Services;
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
        Items = new ObservableCollection<MenuItemRow>();
    }

    public ObservableCollection<MenuItemRow> Items { get; }

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

            LoadMenuItemsAsync(parsed);

            OnPropertyChanged(nameof(BasketTotal));
        }
    }

    private async void LoadMenuItemsAsync(MenuCategory category)
    {
        Items.Clear();

        var items = await _menu.GetByCategoryAsync(category);
        
        foreach (var item in items)
        {
            // Match basket quantity for this item
            var line = _basket.Lines.FirstOrDefault(l => l.Item.Id == item.Id);
            var qty = line?.Quantity ?? 0;

            Items.Add(new MenuItemRow(item, qty));
        }
    }

    public decimal BasketTotal => _basket.Total;

    [RelayCommand]
    private void Add(MenuItemRow row)
    {
        _basket.Add(row.Item);
        RefreshQuantities();
        OnPropertyChanged(nameof(BasketTotal));
    }

    [RelayCommand]
    private void Decrease(MenuItemRow row)
    {
        _basket.Decrease(row.Item);
        RefreshQuantities();
        OnPropertyChanged(nameof(BasketTotal));
    }

    private void RefreshQuantities()
    {
        foreach (var row in Items)
        {
            var line = _basket.Lines.FirstOrDefault(l => l.Item.Id == row.Item.Id);
            row.Quantity = line?.Quantity ?? 0;
        }
    }

    [RelayCommand]
    private async Task CheckoutAsync()
    {
        await Shell.Current.GoToAsync("checkout");
    }
}
