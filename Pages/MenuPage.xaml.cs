using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class MenuPage : ContentPage
{
    public MenuPage(PageModels.MenuPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
