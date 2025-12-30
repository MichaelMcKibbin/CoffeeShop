using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage(CoffeeShop.PageModels.LoginPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

