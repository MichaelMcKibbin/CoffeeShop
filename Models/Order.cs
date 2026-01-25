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

    [Ignore]
    public string FormattedOrderDate
    {
        get
        {
            // Parse OrderNumber format: yyyyMMdd-HHmmss (e.g., "20260125-142126")
            if (string.IsNullOrEmpty(OrderNumber) || OrderNumber.Length < 15)
                return "Unknown date";

            try
            {
                var datePart = OrderNumber.Substring(0, 8);  // yyyyMMdd
                var timePart = OrderNumber.Substring(9, 6);  // HHmmss

                var year = int.Parse(datePart.Substring(0, 4));
                var month = int.Parse(datePart.Substring(4, 2));
                var day = int.Parse(datePart.Substring(6, 2));
                var hour = int.Parse(timePart.Substring(0, 2));
                var minute = int.Parse(timePart.Substring(2, 2));
                var second = int.Parse(timePart.Substring(4, 2));

                var dateTime = new DateTime(year, month, day, hour, minute, second);
                return dateTime.ToString("MMMM dd, yyyy 'at' h:mm tt");
            }
            catch
            {
                // Fallback to LocalDateTime if parsing fails
                return LocalDateTime.ToString("MMMM dd, yyyy 'at' h:mm tt");
            }
        }
    }

    public string Summary =>
        $"Order #{OrderNumber}\n" +
        string.Join("\n", Lines.Select(l => l.ToString())) +
        $"\nTotal: €{Total:0.00}";
}

