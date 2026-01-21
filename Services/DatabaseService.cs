using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using CoffeeShop.Models;

namespace CoffeeShop.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    private async Task Init()
    {
        if (_database != null)
            return;

        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "coffeeshop.db");
        _database = new SQLiteAsyncConnection(databasePath);
        
        await _database.CreateTableAsync<CoffeeShopMenuItem>();
        
        // Seed initial data if table is empty
        var count = await _database.Table<CoffeeShopMenuItem>().CountAsync();
        if (count == 0)
        {
            await SeedInitialData();
        }
    }

    private async Task SeedInitialData()
    {
        var items = new List<CoffeeShopMenuItem>
        {
            new CoffeeShopMenuItem { Name="Americano", Price=3.20m, Category=MenuCategory.HotDrinks },
            new CoffeeShopMenuItem { Name="Cappuccino", Price=3.60m, Category=MenuCategory.HotDrinks },
            new CoffeeShopMenuItem { Name="Tea", Price=2.80m, Category=MenuCategory.HotDrinks },

            new CoffeeShopMenuItem { Name="Ham & Cheese Toastie", Price=5.50m, Category=MenuCategory.Food },
            new CoffeeShopMenuItem { Name="Blueberry Muffin", Price=3.10m, Category=MenuCategory.Food },
            new CoffeeShopMenuItem { Name="Chocolate Brownie", Price=3.40m, Category=MenuCategory.Food },

            new CoffeeShopMenuItem { Name="Water", Price=1.80m, Category=MenuCategory.ColdDrinks },
            new CoffeeShopMenuItem { Name="Canned Cola", Price=2.20m, Category=MenuCategory.ColdDrinks },
            new CoffeeShopMenuItem { Name="Orange Juice", Price=2.50m, Category=MenuCategory.ColdDrinks }
        };

        await _database!.InsertAllAsync(items);
    }

    // Get all menu items
    public async Task<List<CoffeeShopMenuItem>> GetAllMenuItemsAsync()
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>().ToListAsync();
    }

    // Get menu items by category
    public async Task<List<CoffeeShopMenuItem>> GetMenuItemsByCategoryAsync(MenuCategory category)
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>()
            .Where(i => i.Category == category)
            .ToListAsync();
    }

    // Get a single menu item by ID
    public async Task<CoffeeShopMenuItem?> GetMenuItemByIdAsync(string id)
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    // Add a new menu item
    public async Task<int> AddMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        
        // Ensure ID is set
        if (string.IsNullOrEmpty(item.Id))
        {
            item.Id = Guid.NewGuid().ToString("N");
        }
        
        return await _database!.InsertAsync(item);
    }

    // Update an existing menu item
    public async Task<int> UpdateMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        return await _database!.UpdateAsync(item);
    }

    // Delete a menu item
    public async Task<int> DeleteMenuItemAsync(string id)
    {
        await Init();
        var item = await GetMenuItemByIdAsync(id);
        if (item != null)
        {
            return await _database!.DeleteAsync(item);
        }
        return 0;
    }

    // Delete a menu item by object
    public async Task<int> DeleteMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        return await _database!.DeleteAsync(item);
    }
}
