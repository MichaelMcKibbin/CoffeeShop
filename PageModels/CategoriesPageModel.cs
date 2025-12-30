using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels;

public partial class CategoriesPageModel : BaseViewModel
{
    [RelayCommand]
    private async Task OpenCategoryAsync(object parameter)
    {
        // parameter will be "HotDrinks" / "Food" / "ColdDrinks"
        if (parameter is null)
            return;

        if (!Enum.TryParse<MenuCategory>(parameter.ToString(), out var category))
            return;

        await Shell.Current.GoToAsync("menu", new Dictionary<string, object>
        {
            ["Category"] = category.ToString() // send as string to be safe
        });
    }

    [RelayCommand]
    private async Task OpenHistoryAsync()
    {
        await Shell.Current.GoToAsync("history");
    }
}

