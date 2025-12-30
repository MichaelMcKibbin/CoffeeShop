using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Models;

public class Order
{
    public string OrderNumber { get; set; } = "";
    public DateTime LocalDateTime { get; set; } = DateTime.Now;

    public string CustomerName { get; set; } = "";
    public string Telephone { get; set; } = "";

    public List<OrderLine> Lines { get; set; } = new();

    public decimal Total => Lines.Sum(l => l.LineTotal);

    public string Summary =>
        $"Order #{OrderNumber}\n" +
        string.Join("\n", Lines.Select(l => l.ToString())) +
        $"\nTotal: €{Total:0.00}";
}

