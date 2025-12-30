using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;

namespace CoffeeShop.Services;

public class MenuService
{
    private readonly List<CoffeeShopMenuItem> _items = new()
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

    public IReadOnlyList<CoffeeShopMenuItem> GetByCategory(MenuCategory category)
    {
        System.Diagnostics.Debug.WriteLine($"CATEGORY RECEIVED: {category}");
        return _items.Where(i => i.Category == category).ToList();
    }

}

