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
        // Guest users have no order history
        if (_userSession.IsGuestMode || _userSession.CurrentUser == null)
            return new List<Order>();

        return await _database.GetOrdersByUserIdAsync(_userSession.CurrentUser.Id);
    }

    public async Task SaveOrderAsync(Order order)
    {
        // Guest orders are not saved to database
        if (_userSession.IsGuestMode)
        {
            System.Diagnostics.Debug.WriteLine("Guest order not saved to database");
            return;
        }
        
        if (_userSession.CurrentUser == null)
            throw new InvalidOperationException("User must be logged in to save orders");

        order.UserId = _userSession.CurrentUser.Id;
        await _database.SaveOrderAsync(order);
    }

    public async Task<List<Order>> LoadTodayAsync()
    {
        // Guest users have no order history
        if (_userSession.IsGuestMode || _userSession.CurrentUser == null)
            return new List<Order>();

        return await _database.GetTodayOrdersByUserIdAsync(_userSession.CurrentUser.Id);
    }
}



