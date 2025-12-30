using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class CategoriesPage : ContentPage
{
    public CategoriesPage(PageModels.CategoriesPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

