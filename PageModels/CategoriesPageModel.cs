using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Models;
using CoffeeShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels;

public partial class CategoriesPageModel : BaseViewModel
{
    private readonly UserSession _userSession;

    public CategoriesPageModel(UserSession userSession)
    {
        _userSession = userSession;
    }

    public string WelcomeMessage => _userSession.IsGuestMode 
        ? "Welcome, Guest!" 
        : _userSession.CurrentUser != null 
            ? $"Welcome, {_userSession.CurrentUser.FullName}!" 
            : "Welcome!";
    
    public bool CanViewHistory => !_userSession.IsGuestMode;

    [RelayCommand]
    private async Task OpenCategoryAsync(object parameter)
    {
        if (parameter is null)
            return;

        if (!Enum.TryParse<MenuCategory>(parameter.ToString(), out var category))
            return;

        await Shell.Current.GoToAsync("menu", new Dictionary<string, object>
        {
            ["Category"] = category.ToString()
        });
    }

    [RelayCommand]
    private async Task OpenHistoryAsync()
    {
        if (_userSession.IsGuestMode)
        {
            await Shell.Current.DisplayAlertAsync("Not Available", 
                "Order history is only available for registered users. Create an account to save your orders!", 
                "OK");
            return;
        }
        
        await Shell.Current.GoToAsync("history");
    }

    [RelayCommand]
    private async Task OpenSettingsAsync()
    {
        await Shell.Current.GoToAsync("settings");
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        _userSession.Logout();
        await Shell.Current.GoToAsync("//login");
    }

    public void OnAppearing()
    {
        OnPropertyChanged(nameof(WelcomeMessage));
        OnPropertyChanged(nameof(CanViewHistory));
    }
}



