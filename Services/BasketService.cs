using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using System.Collections.ObjectModel;

namespace CoffeeShop.Services;

public class BasketService
{
    public ObservableCollection<OrderLine> Lines { get; } = new();

    public void Add(CoffeeShopMenuItem item)
    {
        var line = Lines.FirstOrDefault(l => l.Item.Id == item.Id);
        if (line is null)
        {
            Lines.Add(new OrderLine { Item = item, Quantity = 1 });
        }
        else
        {
            line.Quantity++;
            // force UI refresh in simple way:
            var idx = Lines.IndexOf(line);
            Lines.RemoveAt(idx);
            Lines.Insert(idx, line);
        }
    }

    public void Decrease(CoffeeShopMenuItem item)
    {
        var line = Lines.FirstOrDefault(l => l.Item.Id == item.Id);
        if (line is null) return;

        line.Quantity--;
        if (line.Quantity <= 0) Lines.Remove(line);
        else
        {
            var idx = Lines.IndexOf(line);
            Lines.RemoveAt(idx);
            Lines.Insert(idx, line);
        }
    }

    public void RemoveLine(OrderLine line)
    {
        if (Lines.Contains(line)) Lines.Remove(line);
    }

    public void Clear() => Lines.Clear();

    public decimal Total => Lines.Sum(l => l.LineTotal);

    public List<OrderLine> SnapshotLines() =>
        Lines.Select(l => new OrderLine { Item = l.Item, Quantity = l.Quantity }).ToList();
}

