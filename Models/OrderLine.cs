using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Models;

public class OrderLine
{
    public CoffeeShopMenuItem Item { get; set; } = new();
    public int Quantity { get; set; }

    public decimal LineTotal => Item.Price * Quantity;

    public override string ToString() => $"{Quantity} x {Item.Name} (€{Item.Price:0.00}) = €{LineTotal:0.00}";
}
