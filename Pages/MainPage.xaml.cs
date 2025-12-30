using CoffeeShop.Models;
using CoffeeShop.PageModels;

namespace CoffeeShop.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}