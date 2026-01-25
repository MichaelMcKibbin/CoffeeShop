using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;

namespace CoffeeShop.Services;

public class OrderStore
{
    private readonly DatabaseService _database;
    private readonly UserSession _userSession;

    public OrderStore(DatabaseService database, UserSession userSession)
    {
        _database = database;
        _userSession = userSession;
    }

    public async Task<List<Order>> LoadAllAsync()
    {
        if (!_userSession.IsLoggedIn)
            return new List<Order>();

        return await _database.GetOrdersByUserIdAsync(_userSession.CurrentUser!.Id);
    }

    public async Task SaveOrderAsync(Order order)
    {
        if (!_userSession.IsLoggedIn)
            throw new InvalidOperationException("User must be logged in to save orders");

        order.UserId = _userSession.CurrentUser!.Id;
        await _database.SaveOrderAsync(order);
    }

    public async Task<List<Order>> LoadTodayAsync()
    {
        if (!_userSession.IsLoggedIn)
            return new List<Order>();

        return await _database.GetTodayOrdersByUserIdAsync(_userSession.CurrentUser!.Id);
    }
}

