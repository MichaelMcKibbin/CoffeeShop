using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;

namespace CoffeeShop.Services;

public class MenuService
{
    private readonly DatabaseService _databaseService;

    public MenuService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IReadOnlyList<CoffeeShopMenuItem>> GetByCategoryAsync(MenuCategory category)
    {
        System.Diagnostics.Debug.WriteLine($"CATEGORY RECEIVED: {category}");
        return await _databaseService.GetMenuItemsByCategoryAsync(category);
    }

    public async Task<IReadOnlyList<CoffeeShopMenuItem>> GetAllItemsAsync()
    {
        return await _databaseService.GetAllMenuItemsAsync();
    }

    public async Task<CoffeeShopMenuItem?> GetItemByIdAsync(string id)
    {
        return await _databaseService.GetMenuItemByIdAsync(id);
    }

    public async Task<int> AddItemAsync(CoffeeShopMenuItem item)
    {
        return await _databaseService.AddMenuItemAsync(item);
    }

    public async Task<int> UpdateItemAsync(CoffeeShopMenuItem item)
    {
        return await _databaseService.UpdateMenuItemAsync(item);
    }

    public async Task<int> DeleteItemAsync(string id)
    {
        return await _databaseService.DeleteMenuItemAsync(id);
    }

    public async Task<int> DeleteItemAsync(CoffeeShopMenuItem item)
    {
        return await _databaseService.DeleteMenuItemAsync(item);
    }
}

