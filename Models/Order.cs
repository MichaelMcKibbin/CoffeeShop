using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace CoffeeShop.Models;

public class Order
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public string OrderNumber { get; set; } = "";
    public DateTime LocalDateTime { get; set; } = DateTime.Now;

    public string CustomerName { get; set; } = "";
    public string Telephone { get; set; } = "";

    [Ignore]
    public List<OrderLine> Lines { get; set; } = new();
    
    // For SQLite storage - serialize order lines as JSON
    public string LinesJson { get; set; } = "[]";

    public decimal Total => Lines.Sum(l => l.LineTotal);

    public string Summary =>
        $"Order #{OrderNumber}\n" +
        string.Join("\n", Lines.Select(l => l.ToString())) +
        $"\nTotal: €{Total:0.00}";
}

