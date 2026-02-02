using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels;

public partial class LoginPageModel : BaseViewModel
{
    private readonly UserSession _userSession;

    public LoginPageModel(UserSession userSession)
    {
        _userSession = userSession;
    }

    [ObservableProperty]
    private string username = "";

    [ObservableProperty]
    private string password = "";

    [ObservableProperty]
    private string fullName = "";

    [ObservableProperty]
    private string phoneNumber = "";

    [ObservableProperty]
    private bool isRegistering = false;

    [ObservableProperty]
    private string errorMessage = "";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    [RelayCommand]
    private async Task SubmitAsync()
    {
        ErrorMessage = "";
        
        (bool success, string message) result;
        
        if (IsRegistering)
        {
            result = await _userSession.RegisterAsync(Username, Password, FullName, PhoneNumber);
        }
        else
        {
            result = await _userSession.LoginAsync(Username, Password);
        }

        if (result.success)
        {
            await Shell.Current.GoToAsync("//categories");
        }
        else
        {
            ErrorMessage = result.message;
            OnPropertyChanged(nameof(HasError));
        }
    }

    [RelayCommand]
    private void ToggleMode()
    {
        IsRegistering = !IsRegistering;
        ErrorMessage = "";
        OnPropertyChanged(nameof(HasError));
    }

    [RelayCommand]
    private async Task ContinueAsGuestAsync()
    {
        _userSession.EnterGuestMode();
        await Shell.Current.GoToAsync("//categories");
    }
}

