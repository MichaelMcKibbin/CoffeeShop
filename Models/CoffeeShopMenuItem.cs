using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CoffeeShop.Models;

public class CoffeeShopMenuItem
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    
    [NotNull]
    public string Name { get; set; } = "";
    
    public decimal Price { get; set; }
    
    public MenuCategory Category { get; set; }
}

