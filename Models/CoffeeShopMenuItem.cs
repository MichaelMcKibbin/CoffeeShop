using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Models;

public class CoffeeShopMenuItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public MenuCategory Category { get; set; }
}

