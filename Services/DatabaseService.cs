using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
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
        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<Order>();
        
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

    #region Menu Items

    public async Task<List<CoffeeShopMenuItem>> GetAllMenuItemsAsync()
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>().ToListAsync();
    }

    public async Task<List<CoffeeShopMenuItem>> GetMenuItemsByCategoryAsync(MenuCategory category)
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>()
            .Where(i => i.Category == category)
            .ToListAsync();
    }

    public async Task<CoffeeShopMenuItem?> GetMenuItemByIdAsync(string id)
    {
        await Init();
        return await _database!.Table<CoffeeShopMenuItem>()
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> AddMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        
        if (string.IsNullOrEmpty(item.Id))
        {
            item.Id = Guid.NewGuid().ToString("N");
        }
        
        return await _database!.InsertAsync(item);
    }

    public async Task<int> UpdateMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        return await _database!.UpdateAsync(item);
    }

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

    public async Task<int> DeleteMenuItemAsync(CoffeeShopMenuItem item)
    {
        await Init();
        return await _database!.DeleteAsync(item);
    }

    #endregion

    #region Users

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        await Init();
        return await _database!.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await Init();
        return await _database!.Table<User>()
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<int> CreateUserAsync(User user)
    {
        await Init();
        user.CreatedAt = DateTime.Now;
        return await _database!.InsertAsync(user);
    }

    public async Task<int> UpdateUserAsync(User user)
    {
        await Init();
        return await _database!.UpdateAsync(user);
    }

    #endregion

    #region Orders

    public async Task<int> SaveOrderAsync(Order order)
    {
        await Init();
        
        // Serialize Lines to JSON for storage
        order.LinesJson = JsonSerializer.Serialize(order.Lines);
        
        return await _database!.InsertAsync(order);
    }

    public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
    {
        await Init();
        var orders = await _database!.Table<Order>()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.LocalDateTime)
            .ToListAsync();
        
        // Deserialize Lines from JSON
        foreach (var order in orders)
        {
            order.Lines = JsonSerializer.Deserialize<List<OrderLine>>(order.LinesJson) ?? new List<OrderLine>();
        }
        
        return orders;
    }

    public async Task<List<Order>> GetTodayOrdersByUserIdAsync(int userId)
    {
        await Init();
        var today = DateTime.Now.Date;
        var orders = await _database!.Table<Order>()
            .Where(o => o.UserId == userId)
            .ToListAsync();
        
        var todayOrders = orders
            .Where(o => o.LocalDateTime.Date == today)
            .OrderByDescending(o => o.LocalDateTime)
            .ToList();
        
        // Deserialize Lines from JSON
        foreach (var order in todayOrders)
        {
            order.Lines = JsonSerializer.Deserialize<List<OrderLine>>(order.LinesJson) ?? new List<OrderLine>();
        }
        
        return todayOrders;
    }

    public async Task<Order?> GetOrderByIdAsync(int id)
    {
        await Init();
        var order = await _database!.Table<Order>()
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync();
        
        if (order != null)
        {
            order.Lines = JsonSerializer.Deserialize<List<OrderLine>>(order.LinesJson) ?? new List<OrderLine>();
        }
        
        return order;
    }

    #endregion
}
