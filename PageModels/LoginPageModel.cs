using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Input;

namespace CoffeeShop.PageModels;

public partial class LoginPageModel : BaseViewModel
{
    [RelayCommand]
    private async Task EnterAsync()
    {
        await Shell.Current.GoToAsync("//categories");
    }
}

