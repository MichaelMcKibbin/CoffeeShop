using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class HistoryPage : ContentPage
{
    private readonly PageModels.HistoryPageModel _vm;

    public HistoryPage(PageModels.HistoryPageModel vm)
    {
        InitializeComponent();
        _vm = vm;
        BindingContext = _vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}

