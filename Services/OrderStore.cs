using System;
using System.Collections.Generic;
using System.Text;

using CoffeeShop.Models;
using System.Text.Json;

namespace CoffeeShop.Services;

public class OrderStore
{
    private readonly string _path;

    public OrderStore()
    {
        _path = Path.Combine(FileSystem.AppDataDirectory, "orders.json");
    }

    public async Task<List<Order>> LoadAllAsync()
    {
        if (!File.Exists(_path)) return new List<Order>();
        var json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<List<Order>>(json) ?? new List<Order>();
    }

    public async Task SaveOrderAsync(Order order)
    {
        var all = await LoadAllAsync();
        all.Add(order);

        var json = JsonSerializer.Serialize(all, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_path, json);
    }

    public async Task<List<Order>> LoadTodayAsync()
    {
        var all = await LoadAllAsync();
        var today = DateTime.Now.Date;
        return all.Where(o => o.LocalDateTime.Date == today)
                  .OrderByDescending(o => o.LocalDateTime)
                  .ToList();
    }
}

