using System;
using System.Collections.Generic;
using System.Text;
using CoffeeShop.PageModels;

namespace CoffeeShop.Pages;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesPageModel _viewModel;

    public CategoriesPage(CategoriesPageModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }
}

