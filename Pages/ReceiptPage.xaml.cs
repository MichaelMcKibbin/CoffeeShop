using System;
using System.Collections.Generic;
using System.Text;

namespace CoffeeShop.Pages;

public partial class ReceiptPage : ContentPage
{
    public ReceiptPage(PageModels.ReceiptPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

