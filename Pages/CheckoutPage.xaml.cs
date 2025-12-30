using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class CheckoutPage : ContentPage
{
    public CheckoutPage(CoffeeShop.PageModels.CheckoutPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

